using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class MoMoService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public MoMoService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task<(bool success, string payUrl, string id, string message)> CreatePaymentAsync(decimal amount, string orderId, string orderInfo, string returnUrl, string notifyUrl)
        {
            string endpoint = _config["MoMo:Endpoint"];
            string partnerCode = _config["MoMo:PartnerCode"];
            string accessKey = _config["MoMo:AccessKey"];
            string secretKey = _config["MoMo:SecretKey"];

            var requestId = Guid.NewGuid().ToString();
            var orderIdFull = $"{orderId}_{DateTime.Now.Ticks}";
            var rawHash = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={notifyUrl}&orderId={orderIdFull}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType=captureWallet";

            string signature = HmacSHA256(rawHash, secretKey);

            var payload = new
            {
                partnerCode,
                partnerName = "Test",
                storeId = "MomoTestStore",
                requestId,
                amount = amount.ToString(),
                orderId = orderIdFull,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                requestType = "captureWallet",
                extraData = "",
                lang = "vi",
                signature
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
            return (true, json["payUrl"].ToString(), orderIdFull, "OK");
        }

        private static string HmacSHA256(string message, string secret)
        {
            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
