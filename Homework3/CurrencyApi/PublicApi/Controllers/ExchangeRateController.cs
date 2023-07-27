using currencyapi;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Controllers
{
    /// <summary>
    /// Методы получения курсов валют
    /// </summary>
    [Route("ExchangeRate")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ILimitCheck _limitCheck;
        private readonly ISendMessage _sendMessage;
        public ExchangeRateController(ILimitCheck limitCheck, ISendMessage sendMessage)
        {
            _limitCheck = limitCheck;
            _sendMessage = sendMessage;
        }

        //private readonly IConfiguration _configuration;
        //private readonly ILogger<ExchangeRateController> _logger;
        //public ExchangeRateController(IConfiguration configuration, ILogger<ExchangeRateController> logger, ILimitCheck limitCheck, ISendMessage sendMessage)
        //{
        //    _configuration = configuration;
        //    _logger = logger;
        //    _limitCheck = limitCheck;
        //    _sendMessage = sendMessage;
        //}

        /// <summary>
        /// Метод получения курса валюты по умолчанию.
        /// </summary>
        /// <returns>
        /// Возвращает JSON вида
        ///        {
        ///  "code": "RUB", // код валюты
        ///  "value": 90.50 // текущий курс относительно доллара
        ///}
        /// </returns>
        [HttpGet]
        [Route("Currency")]
        public async Task<string> Currency()
        {
            if (_limitCheck.CheckLimit())
                throw new ApiRequestLimitException("Превышен лимит запросов.");
            else
                return await _sendMessage.SendMessageAsync();
        }

        //[HttpGet]
        //[Route("Currency/{currencyCode}")]
        //public async Task<string> Currency(string currencyCode)
        //{
        //    var apiKey = _configuration["Settings:APIKey"];
        //    var baseCurrencyCode = _configuration["Currency:Base"];
        //    int.TryParse(_configuration["Currency:Round"], out int round);
        //    var path = $"https://api.currencyapi.com/v3/latest?currencies={currencyCode}&base_currency={baseCurrencyCode}";

        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Clear();
        //    var message = new HttpRequestMessage();
        //    message.Headers.Add("apikey", apiKey);
        //    message.RequestUri = new Uri(path);
        //    message.Method = HttpMethod.Get;

        //    HttpResponseMessage apiResponse = new();
        //    apiResponse = await client.SendAsync(message);
        //    if (!apiResponse.IsSuccessStatusCode)
        //    {
        //        return HttpStatusCode.NotFound.ToString();
        //    }
        //    var apiContent = await apiResponse.Content.ReadAsStringAsync();
        //    var data = JObject.Parse(apiContent);
        //    var currency = new Currency();
        //    if (data != null)
        //    {
        //        currency.Code = Convert.ToString(data["data"][currencyCode]["code"]);
        //        currency.Value = Math.Round(Convert.ToDecimal(data["data"][currencyCode]["value"]), round);
        //    }
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = new LowerCaseNamingPolicy(),
        //        WriteIndented = true
        //    };
        //    string jsonString = JsonSerializer.Serialize(currency, options);

        //    return jsonString;
        //}

        //[HttpGet]
        //[Route("Currency/{currencyCode}/{date}")]
        //public async Task<string> Currency(string currencyCode, DateTime date)
        //{
        //    var dateString = date.ToString("yyyy-MM-dd");

        //    var apiKey = _configuration["Settings:APIKey"];
        //    var baseCurrencyCode = _configuration["Currency:Base"];
        //    int.TryParse(_configuration["Currency:Round"], out int round);
        //    var path = $"https://api.currencyapi.com/v3/historical?currencies={currencyCode}&date={dateString}&base_currency={baseCurrencyCode}";

        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Clear();
        //    var message = new HttpRequestMessage();
        //    message.Headers.Add("apikey", apiKey);
        //    message.RequestUri = new Uri(path);
        //    message.Method = HttpMethod.Get;

        //    HttpResponseMessage apiResponse = new();
        //    apiResponse = await client.SendAsync(message);
        //    var apiContent = await apiResponse.Content.ReadAsStringAsync();
        //    var data = JObject.Parse(apiContent);
        //    var currencyWithDate = new CurrencyWithDate();
        //    if (data != null)
        //    {
        //        currencyWithDate.Date = dateString;
        //        currencyWithDate.Code = Convert.ToString(data["data"][currencyCode]["code"]);
        //        currencyWithDate.Value = Math.Round(Convert.ToDecimal(data["data"][currencyCode]["value"]), round);
        //    }
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = new LowerCaseNamingPolicy(),
        //        WriteIndented = true
        //    };
        //    string jsonString = JsonSerializer.Serialize(currencyWithDate, options);

        //    return jsonString;
        //}

        //[HttpGet]
        //[Route("settings")]
        //public async Task<string> Settings()
        //{
        //    var apiKey = _configuration["Settings:APIKey"];
        //    var defaultCurrencyCode = _configuration["Currency:Default"];
        //    var baseCurrencyCode = _configuration["Currency:Base"];
        //    int.TryParse(_configuration["Currency:Round"], out int round);
        //    var path = $"https://api.currencyapi.com/v3/status";

        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Clear();
        //    var message = new HttpRequestMessage();
        //    message.Headers.Add("apikey", apiKey);
        //    message.RequestUri = new Uri(path);
        //    message.Method = HttpMethod.Get;

        //    HttpResponseMessage apiResponse = new();
        //    apiResponse = await client.SendAsync(message);
        //    var apiContent = await apiResponse.Content.ReadAsStringAsync();
        //    var data = JObject.Parse(apiContent);
        //    var settings = new Settings();
        //    if (data != null)
        //    {
        //        settings.DefaultCurrency = defaultCurrencyCode;
        //        settings.BaseCurrency = baseCurrencyCode;
        //        int.TryParse(Convert.ToString(data["quotas"]["month"]["total"]), out int limit);
        //        int.TryParse(Convert.ToString(data["quotas"]["month"]["used"]), out int used);
        //        settings.RequestLimit = limit;
        //        settings.RequestCount = used;
        //        settings.CurrencyRoundCount = round;
        //    }
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //        WriteIndented = true
        //    };
        //    string jsonString = JsonSerializer.Serialize(settings, options);

        //    return jsonString;
        //}
    }
}
