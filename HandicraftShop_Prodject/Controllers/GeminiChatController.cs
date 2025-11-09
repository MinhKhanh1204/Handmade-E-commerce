using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandicraftShop_Prodject.Controllers
{
    public class GeminiChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GeminiChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("message", out var messageElement))
                return BadRequest(new { reply = "❌ Message is missing" });

            string? message = messageElement.GetString();
            if (string.IsNullOrEmpty(message))
                return BadRequest(new { reply = "❌ Message is empty" });

            string apiKey = _configuration["Gemini:ApiKey"] ?? "";
            if (string.IsNullOrEmpty(apiKey))
                return Json(new { reply = "⚠️ API Key chưa được cấu hình. Vui lòng thêm 'Gemini:ApiKey' vào appsettings.json" });

            // Thử các model names phổ biến, ưu tiên model mới nhất
            string[] modelNames = { "gemini-pro", "gemini-1.5-pro", "gemini-1.5-flash", "gemini-2.0-flash" };
            string? lastError = null;
            
            foreach (var modelName in modelNames)
            {
                try
                {
                    string url = $"https://generativelanguage.googleapis.com/v1/models/{modelName}:generateContent?key={apiKey}";
                    var result = await TrySendRequest(url, message);
                    if (result != null)
                        return Json(new { reply = result });
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    continue; // Thử model tiếp theo
                }
            }
            
            // Nếu tất cả đều thất bại, thử với v1beta
            try
            {
                string urlBeta = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";
                var result = await TrySendRequest(urlBeta, message);
                if (result != null)
                    return Json(new { reply = result });
            }
            catch
            {
                // Ignore
            }
            
            return Json(new { reply = $"⚠️ Không thể kết nối với Gemini API. Lỗi: {lastError ?? "Không tìm thấy model phù hợp"}" });
        }

        private async Task<string?> TrySendRequest(string url, string message)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new {
                        parts = new[]
                        {
                            new { text = message }
                        }
                    }
                }
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
            
            // Xử lý response từ Gemini API
            string? reply = null;
            
            try
            {
                if (result != null)
                {
                    var candidates = result.candidates;
                    if (candidates != null)
                    {
                        int count = candidates.Count;
                        if (count > 0)
                        {
                            var candidate = candidates[0];
                            if (candidate != null)
                            {
                                var candidateContent = candidate.content;
                                if (candidateContent != null)
                                {
                                    var parts = candidateContent.parts;
                                    if (parts != null)
                                    {
                                        int partsCount = parts.Count;
                                        if (partsCount > 0)
                                        {
                                            var part = parts[0];
                                            if (part != null)
                                            {
                                                reply = part.text?.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Nếu parse lỗi, return null để thử model khác
                return null;
            }

            if (string.IsNullOrEmpty(reply))
                return null;

            return reply;
        }

    }
}
