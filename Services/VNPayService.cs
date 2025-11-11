using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Services
{
    public class VNPayService
    {
        private readonly IConfiguration _config;

        public VNPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string returnUrl, string clientIp = "127.0.0.1")
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string vnp_Url = _config["VNPay:BaseUrl"]?.Trim();
            string vnp_TmnCode = _config["VNPay:TmnCode"]?.Trim();
            string vnp_HashSecret = _config["VNPay:HashSecret"]?.Trim();

            if (string.IsNullOrEmpty(vnp_Url) || string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                throw new Exception("VNPay configuration is missing!");
            }

            // Timezone Việt Nam (UTC+7)
            var vnpTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vnpTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnpTimeZone);

            var vnp_Amount = ((long)(amount * 100)).ToString();
            var vnp_TxnRef = orderId;
            var vnp_OrderInfo = $"Thanh toan don hang {orderId}"; // ✅ Có dấu cách
            var vnp_CreateDate = vnpTime.ToString("yyyyMMddHHmmss");

            // ✅ Khai báo đầy đủ tất cả tham số theo VNPay
            var vnp_Params = new SortedList<string, string>
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", vnp_TmnCode},
                {"vnp_Amount", vnp_Amount},
                {"vnp_CreateDate", vnp_CreateDate},
                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", clientIp},
                {"vnp_Locale", "vn"},
                {"vnp_OrderInfo", vnp_OrderInfo},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", returnUrl},
                {"vnp_TxnRef", vnp_TxnRef}
            };

            // ✅ Tạo rawData để ký - encode và thay %20 thành +
            var signData = new StringBuilder();
            foreach (var kvp in vnp_Params)
            {
                if (signData.Length > 0)
                    signData.Append('&');

                signData.Append(Uri.EscapeDataString(kvp.Key));
                signData.Append('=');
                signData.Append(Uri.EscapeDataString(kvp.Value).Replace("%20", "+")); // ✅ Key point!
            }

            var rawData = signData.ToString();
            var secureHash = HmacSHA512(vnp_HashSecret, rawData);

            // ✅ Tạo URL cuối cùng
            var finalUrl = $"{vnp_Url}?{rawData}&vnp_SecureHash={secureHash}";

            Console.WriteLine("=== VNPay CREATE PAYMENT DEBUG ===");
            Console.WriteLine($"TmnCode: {vnp_TmnCode}");
            Console.WriteLine($"CreateDate: {vnp_CreateDate}");
            Console.WriteLine($"OrderInfo: {vnp_OrderInfo}");
            Console.WriteLine($"RawData: {rawData}");
            Console.WriteLine($"SecureHash: {secureHash}");
            Console.WriteLine("===================================");

            return finalUrl;
        }

        public bool ValidateSignature(IQueryCollection queryParams, string inputHash)
        {
            string vnp_HashSecret = _config["VNPay:HashSecret"]?.Trim();

            if (string.IsNullOrEmpty(vnp_HashSecret))
            {
                Console.WriteLine("❌ VNPay HashSecret is missing!");
                return false;
            }

            var vnp_Params = new SortedList<string, string>();

            // Lấy tất cả params trừ vnp_SecureHash
            foreach (var key in queryParams.Keys)
            {
                var value = queryParams[key].ToString();
                if (!string.IsNullOrEmpty(value) &&
                    key != "vnp_SecureHash" &&
                    key != "vnp_SecureHashType")
                {
                    vnp_Params.Add(key, value);
                }
            }

            // ✅ Tạo rawData giống hệt lúc tạo payment
            var signData = new StringBuilder();
            foreach (var kvp in vnp_Params)
            {
                if (signData.Length > 0)
                    signData.Append('&');

                signData.Append(Uri.EscapeDataString(kvp.Key));
                signData.Append('=');
                signData.Append(Uri.EscapeDataString(kvp.Value).Replace("%20", "+")); // ✅ Key point!
            }

            var rawData = signData.ToString();
            var computedHash = HmacSHA512(vnp_HashSecret, rawData);

            Console.WriteLine("=== VNPay CALLBACK VALIDATION ===");
            Console.WriteLine($"RawData: {rawData}");
            Console.WriteLine($"Computed: {computedHash}");
            Console.WriteLine($"Received: {inputHash}");
            Console.WriteLine($"Match: {computedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase)}");
            Console.WriteLine("==================================");

            return computedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }

            if (ipAddress == "::1" || ipAddress?.StartsWith("::ffff:") == true)
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress ?? "127.0.0.1";
        }

        private static string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}