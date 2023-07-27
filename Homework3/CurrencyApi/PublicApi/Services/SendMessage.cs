using Audit.Core;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    public class SendMessage : ISendMessage
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendMessage> _logger;
        private readonly HttpClient _httpClient;

        public SendMessage(IConfiguration configuration, ILogger<SendMessage> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<string> SendMessageAsync()
        {
            var apiKey = _configuration["Settings:APIKey"];
            var defaultCurrencyCode = _configuration["Currency:Default"];
            var baseCurrencyCode = _configuration["Currency:Base"];
            int.TryParse(_configuration["Currency:Round"], out int round);
            var path = $"https://api.currencyapi.com/v3/latest?currencies={defaultCurrencyCode}&base_currency={baseCurrencyCode}";


            _httpClient.DefaultRequestHeaders.Clear();
            var message = new HttpRequestMessage();
            message.Headers.Add("apikey", apiKey);
            message.RequestUri = new Uri(path);
            message.Method = HttpMethod.Get;

            //Логирование через Audit.Net
            var auditEvent = new HttpAction
            {
                RequestBody = message.Content.ToString(),
                RequestHeaders = message.Headers.ToString(),
            };
            AuditScope.Log("", auditEvent);

            HttpResponseMessage apiResponse = new();
            apiResponse = await _httpClient.SendAsync(message);

            //Логирование через Audit.Net
            auditEvent.ResponseBody = apiResponse.Content.ToString();
            auditEvent.ResponseHeaders = apiResponse.Headers.ToString();
            auditEvent.ContentHeader = apiResponse.Content.Headers.ToString();
            AuditScope.Log("", auditEvent);

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var data = JObject.Parse(apiContent);
            var currency = new Currency();
            if (data != null)
            {
                currency.Code = Convert.ToString(data["data"]["RUB"]["code"]);
                currency.Value = Math.Round(Convert.ToDecimal(data["data"]["RUB"]["value"]), round);
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(currency, options);

            return jsonString;
        }
    }
}
