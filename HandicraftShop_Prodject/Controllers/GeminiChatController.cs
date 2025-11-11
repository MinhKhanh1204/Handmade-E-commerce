using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Services;
using DTO;
using System.Linq;
using System;
using System.Collections.Generic;

namespace HandicraftShop_Prodject.Controllers
{
    public class GeminiChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IProductService _productService;
        private const string ChatSessionKey = "ChatHistory";

        public GeminiChatController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IProductService productService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("message", out var messageElement))
                return BadRequest(new { reply = "❌ Tin nhắn trống" });

            string? message = messageElement.GetString();
            if (string.IsNullOrEmpty(message))
                return BadRequest(new { reply = "❌ Tin nhắn trống" });

            var allProducts = _productService.GetProductDTOs();
            string lowerMessage = message.ToLower();

            // 1️⃣ Lấy lịch sử chat
            List<string> chatHistory = HttpContext.Session.GetObject<List<string>>(ChatSessionKey) ?? new List<string>();
            chatHistory.Add($"Khách hàng: {message}");

            // 2️⃣ Kiểm tra các yêu cầu đặc biệt
            // - Sản phẩm giảm giá
            if (lowerMessage.Contains("giảm giá") || lowerMessage.Contains("sale") || lowerMessage.Contains("discount"))
            {
                var discountProducts = allProducts
                    .Where(p => p.Discount.HasValue && p.Discount.Value > 0)
                    .Select(p => p.ProductName)
                    .ToList();

                string reply = discountProducts.Any()
                    ? $"Hiện tại shop có những sản phẩm đang giảm giá như: {string.Join(", ", discountProducts)}. Bạn có muốn mình tư vấn chi tiết không?"
                    : "Hiện tại shop chưa có sản phẩm nào đang giảm giá.";

                chatHistory.Add($"Bot: {reply}");
                HttpContext.Session.SetObject(ChatSessionKey, chatHistory);
                return Json(new { reply });
            }

            // 3️⃣ Tạo reference sản phẩm cho Gemini
            var productReference = allProducts
                .Select(p => $"{p.ProductName}: {p.Description}")
                .Take(50);
            string dbReference = string.Join("; ", productReference);

            // 4️⃣ Tạo prompt kèm lịch sử chat
            string historyText = string.Join("\n", chatHistory);
            string prompt = $"Lịch sử trò chuyện:\n{historyText}\n" +
                            $"Thông tin sản phẩm shop hiện có (tham khảo): {dbReference}\n" +
                            "Hãy trả lời thân thiện, gợi ý sản phẩm phù hợp mà không nhắc giá, ID hay URL. " +
                            "Có thể tư vấn thêm nếu khách chưa biết muốn gì. Trả lời bằng tiếng Việt khoảng 3 đến 5 câu.";

            // 5️⃣ Gọi Gemini API
            string apiKey = _configuration["Gemini:ApiKey"] ?? "";
            if (string.IsNullOrEmpty(apiKey))
                return Json(new { reply = "⚠️ API Key chưa được cấu hình. Vui lòng thêm 'Gemini:ApiKey' vào appsettings.json" });

            string[] modelNames = { "gemini-pro", "gemini-1.5-pro", "gemini-1.5-flash", "gemini-2.0-flash" };
            string? lastError = null;
            string? replyText = null;

            foreach (var modelName in modelNames)
            {
                try
                {
                    string url = $"https://generativelanguage.googleapis.com/v1/models/{modelName}:generateContent?key={apiKey}";
                    var result = await TrySendRequest(url, prompt);
                    if (!string.IsNullOrEmpty(result))
                    {
                        replyText = result;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    continue;
                }
            }

            // fallback v1beta
            if (string.IsNullOrEmpty(replyText))
            {
                try
                {
                    string urlBeta = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";
                    var result = await TrySendRequest(urlBeta, prompt);
                    if (!string.IsNullOrEmpty(result))
                        replyText = result;
                }
                catch { }
            }

            // fallback động nếu Gemini lỗi
            if (string.IsNullOrEmpty(replyText))
                replyText = GenerateDynamicFallback(message, allProducts);

            chatHistory.Add($"Bot: {replyText}");
            HttpContext.Session.SetObject(ChatSessionKey, chatHistory);

            return Json(new { reply = replyText });
        }

        private string GenerateDynamicFallback(string message, List<ProductDTO> allProducts)
        {
            string lowerMessage = message.ToLower();

            var matchedProducts = allProducts
                .Where(p => !string.IsNullOrEmpty(p.ProductName) && p.ProductName!.ToLower().Contains(lowerMessage))
                .Select(p => p.ProductName)
                .ToList();

            if (matchedProducts.Count > 0)
            {
                return $"Mình thấy bạn có thể quan tâm đến những sản phẩm như: {string.Join(", ", matchedProducts)}. " +
                       "Bạn có muốn mình tư vấn chi tiết hơn về những sản phẩm này không?";
            }

            var categories = allProducts
                .Select(p => p.CategoryName)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct();

            return $"Chào bạn! Shop hiện có các loại sản phẩm: {string.Join(", ", categories)}. " +
                   "Bạn đang tìm sản phẩm cho mục đích gì hoặc thích chất liệu nào? Mình sẽ gợi ý những sản phẩm phù hợp.";
        }

        private async Task<string?> TrySendRequest(string url, string prompt)
        {
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP {response.StatusCode}: {json}");
            }

            dynamic? result = JsonConvert.DeserializeObject(json);
            try
            {
                var candidates = result?.candidates;
                if (candidates != null && candidates.Count > 0)
                {
                    var part = candidates[0]?.content?.parts?[0];
                    return part?.text?.ToString();
                }
            }
            catch { return null; }

            return null;
        }
    }

    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
            => session.SetString(key, JsonConvert.SerializeObject(value));

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
