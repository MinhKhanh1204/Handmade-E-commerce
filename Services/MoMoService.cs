using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

public class MoMoService
{
    private readonly IConfiguration _config;
    public MoMoService(IConfiguration config) => _config = config;

   public async Task<(bool Success, string PayUrl, string OrderId, string Message)> CreatePaymentAsync(
    decimal amount, string orderId, string orderInfo, string returnUrl, string notifyUrl)
{
    var partnerCode = _config["MoMo:PartnerCode"];
    var accessKey = _config["MoMo:AccessKey"];
    var secretKey = _config["MoMo:SecretKey"];
    var endpoint = _config["MoMo:Endpoint"];

    var requestId = Guid.NewGuid().ToString();
    var requestType = "captureMoMoWallet";
    var extraData = "";

    // ✅ MoMo yêu cầu amount phải là kiểu long, không có .0000
    var amountStr = ((long)amount).ToString();

    // ✅ Chuỗi rawHash đúng chuẩn API captureMoMoWallet
    var rawHash =
        $"partnerCode={partnerCode}" +
        $"&accessKey={accessKey}" +
        $"&requestId={requestId}" +
        $"&amount={amountStr}" +
        $"&orderId={orderId}" +
        $"&orderInfo={orderInfo}" +
        $"&returnUrl={returnUrl}" +
        $"&notifyUrl={notifyUrl}" +
        $"&extraData={extraData}";

    var signature = HmacSHA256(secretKey, rawHash);

    Console.WriteLine("=== MoMo DEBUG ===");
    Console.WriteLine($"RawHash: {rawHash}");
    Console.WriteLine($"Signature: {signature}");
    Console.WriteLine("==================");

    var payload = new
    {
        partnerCode,
        accessKey,
        requestId,
        amount = amountStr,
        orderId,
        orderInfo,
        returnUrl,
        notifyUrl,
        extraData,
        requestType,
        signature,
        lang = "vi"
    };

    using var client = new HttpClient();
    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    var response = await client.PostAsync(endpoint, content);
    var responseBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        return (false, "", orderId, responseBody);

    var json = System.Text.Json.JsonDocument.Parse(responseBody);
    var payUrl = json.RootElement.GetProperty("payUrl").GetString();

    return (true, payUrl!, orderId, "OK");
}

private static string HmacSHA256(string key, string message)
{
    var keyBytes = Encoding.UTF8.GetBytes(key);
    var messageBytes = Encoding.UTF8.GetBytes(message);
    using var hmac = new HMACSHA256(keyBytes);
    var hashBytes = hmac.ComputeHash(messageBytes);
    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
}

}
