using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace Services
{
    public class VNPayService
    {
        private readonly IConfiguration _config;

        public VNPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string returnUrl)
        {
            string vnp_Url = _config["VNPay:BaseUrl"];
            string vnp_TmnCode = _config["VNPay:TmnCode"];
            string vnp_HashSecret = _config["VNPay:HashSecret"];

            var vnp_Amount = ((long)(amount * 100)).ToString();
            var vnp_TxnRef = $"{orderId}_{DateTime.Now.Ticks}";
            var vnp_OrderInfo = $"Thanh toán đơn hàng {orderId}";

            var vnp_Params = new SortedDictionary<string, string>
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", vnp_TmnCode},
                {"vnp_Amount", vnp_Amount},
                {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
                {"vnp_CurrCode", "VND"},
                {"vnp_IpAddr", "127.0.0.1"},
                {"vnp_Locale", "vn"},
                {"vnp_OrderInfo", vnp_OrderInfo},
                {"vnp_OrderType", "other"},
                {"vnp_ReturnUrl", returnUrl},
                {"vnp_TxnRef", vnp_TxnRef}
            };

            var rawData = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            var secureHash = HmacSHA512(vnp_HashSecret, rawData);

            var query = new StringBuilder();
            foreach (var kvp in vnp_Params)
            {
                query.Append(Uri.EscapeDataString(kvp.Key));
                query.Append('=');
                query.Append(Uri.EscapeDataString(kvp.Value));
                query.Append('&');
            }

            query.Append("vnp_SecureHash=");
            query.Append(secureHash);

            return $"{vnp_Url}?{query}";
        }

        private static string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
