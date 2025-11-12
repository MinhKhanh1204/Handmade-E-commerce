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

            var allProducts = _productService.GetProductDTOs() ?? new List<ProductDTO>();
            string lowerMessage = message.ToLower();

            // 1️⃣ Lấy lịch sử chat
            List<string> chatHistory = HttpContext.Session.GetObject<List<string>>(ChatSessionKey) ?? new List<string>();
            chatHistory.Add($"Khách hàng: {message}");

            // 2️⃣ Kiểm tra yêu cầu đặc biệt
            // - Nếu hỏi giảm giá
            if (lowerMessage.Contains("giảm giá") || lowerMessage.Contains("sale") || lowerMessage.Contains("discount"))
            {
                var discountProducts = allProducts
                    .Where(p => p.Discount.HasValue && p.Discount.Value > 0)
                    .Select(p => p.ProductName)
                    .ToList();

                string reply = discountProducts.Any()
                    ? $"Hiện tại shop có những sản phẩm đang giảm giá: {string.Join(", ", discountProducts)}. Bạn có muốn mình tư vấn chi tiết không?"
                    : "Hiện tại shop chưa có sản phẩm nào đang giảm giá.";

                chatHistory.Add($"Bot: {reply}");
                HttpContext.Session.SetObject(ChatSessionKey, chatHistory);
                return Json(new { reply });
            }

            // 3️⃣ Gợi ý sản phẩm theo chủ đề khách hỏi
            var topicMatchedProducts = allProducts
                .Where(p => !string.IsNullOrEmpty(p.ProductName) &&
                            (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(lowerMessage) ||
                             !string.IsNullOrEmpty(p.CategoryName) && p.CategoryName.ToLower().Contains(lowerMessage)))
                .Take(10) // lấy tối đa 10 sản phẩm
                .Select(p => new {
                    p.ProductName,
                    p.CategoryName,
                    p.Description,
                    p.Price,
                    p.Discount
                })
                .ToList();

            if (topicMatchedProducts.Any())
            {
                string reply = "Mình tìm thấy những sản phẩm phù hợp với yêu cầu của bạn:\n";
                foreach (var p in topicMatchedProducts)
                {
                    decimal finalPrice = p.Price.HasValue
                             ? (p.Discount.HasValue ? p.Price.Value * (1 - p.Discount.Value / 100m) : p.Price.Value)
                                : 0m; // hoặc 1 giá trị mặc định

                    reply += $"- {p.ProductName} ({p.CategoryName}): {p.Description}. Giá: {finalPrice:N0} VNĐ\n";
                }

                chatHistory.Add($"Bot: {reply}");
                HttpContext.Session.SetObject(ChatSessionKey, chatHistory);
                return Json(new { reply });
            }

            // 4️⃣ Nếu không tìm thấy sản phẩm nào, fallback cho Gemini AI
            string fallbackPrompt = $"Khách hỏi: {message}\n" +
                                    $"Thông tin sản phẩm shop hiện có (tham khảo): {string.Join("; ", allProducts.Take(50).Select(p => $"{p.ProductName}: {p.Description}"))}\n" +
                                    "Trả lời thân thiện, gợi ý sản phẩm phù hợp mà không nhắc giá, ID hay URL. Tiếng Việt.";

            string apiKey = _configuration["Gemini:ApiKey"] ?? "";
            string replyText = string.IsNullOrEmpty(apiKey) ? "⚠️ API Key chưa được cấu hình" : await CallGeminiAPI(fallbackPrompt);

            chatHistory.Add($"Bot: {replyText}");
            HttpContext.Session.SetObject(ChatSessionKey, chatHistory);

            return Json(new { reply = replyText });
        }

        private async Task<string> CallGeminiAPI(string prompt)
        {
            string apiKey = _configuration["Gemini:ApiKey"] ?? "";
            string url = $"https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key={apiKey}";

            try
            {
                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var httpClient = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode) return "Xin lỗi, bot đang bận, vui lòng thử lại sau.";

                var json = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(json);
                var candidates = result?.candidates;
                if (candidates != null && candidates.Count > 0)
                    return candidates[0]?.content?.parts?[0]?.text?.ToString() ?? "Xin lỗi, bot không trả lời được.";

                return "Xin lỗi, bot không trả lời được.";
            }
            catch
            {
                return "Xin lỗi, bot đang gặp lỗi, vui lòng thử lại sau.";
            }
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
