using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    /// <summary>
    /// Сервис для работы с currencyApi
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public CurrencyRateService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<Currency> GetCurrencyAsync()
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

            HttpResponseMessage apiResponse = apiResponse = await _httpClient.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            var currency = new Currency();
            if (dataApiContent.data.ContainsKey(defaultCurrencyCode))
            {
                var data = dataApiContent.data[defaultCurrencyCode];
                currency.Code = data.code;
                currency.Value = Math.Round(data.value, round);

                return currency;
            }
            else
                throw new Exception();
        }

        public async Task<Currency> GetCurrencyAsync(string currencyCode)
        {
            var apiKey = _configuration["Settings:APIKey"];
            var baseCurrencyCode = _configuration["Currency:Base"];
            int.TryParse(_configuration["Currency:Round"], out int round);
            var path = $"https://api.currencyapi.com/v3/latest?currencies={currencyCode}&base_currency={baseCurrencyCode}";

            _httpClient.DefaultRequestHeaders.Clear();
            var message = new HttpRequestMessage();

            message.Headers.Add("apikey", apiKey);
            message.RequestUri = new Uri(path);
            message.Method = HttpMethod.Get;

            HttpResponseMessage apiResponse = await _httpClient.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            var currency = new Currency();
            if (dataApiContent.data != null && dataApiContent.data.ContainsKey(currencyCode))
            {
                var data = dataApiContent.data[currencyCode];
                currency.Code = data.code;
                currency.Value = Math.Round(data.value, round);

                return currency;
            }
            if (apiResponse.StatusCode.ToString() == "UnprocessableEntity")
                throw new CurrencyNotFoundException("Нет запрошенной валюты!");
            else
                throw new Exception();
        }

        public async Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date)
        {
            var dateString = date.ToString("yyyy-MM-dd");

            var apiKey = _configuration["Settings:APIKey"];
            var baseCurrencyCode = _configuration["Currency:Base"];
            int.TryParse(_configuration["Currency:Round"], out int round);
            var path = $"https://api.currencyapi.com/v3/historical?currencies={currencyCode}&date={dateString}&base_currency={baseCurrencyCode}";

            _httpClient.DefaultRequestHeaders.Clear();
            var message = new HttpRequestMessage();

            message.Headers.Add("apikey", apiKey);
            message.RequestUri = new Uri(path);
            message.Method = HttpMethod.Get;

            HttpResponseMessage apiResponse = apiResponse = await _httpClient.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            var currencyWithDate = new CurrencyWithDate();
            if (dataApiContent.data != null && dataApiContent.data.ContainsKey(currencyCode))
            {
                var data = dataApiContent.data[currencyCode];
                currencyWithDate.Date = dataApiContent.meta.last_updated_at.ToString("yyyy-MM-dd");
                currencyWithDate.Code = data.code;
                currencyWithDate.Value = Math.Round(data.value, round);

                return currencyWithDate;
            }
            if (apiResponse.StatusCode.ToString() == "UnprocessableEntity")
                throw new CurrencyNotFoundException("Нет запрошенной валюты!");
            else
                throw new Exception();
        }

        public async Task<CurrencySettings> GetCurrencySettingsAsync()
        {
            var apiKey = _configuration["Settings:APIKey"];
            var defaultCurrencyCode = _configuration["Currency:Default"];
            var baseCurrencyCode = _configuration["Currency:Base"];
            int.TryParse(_configuration["Currency:Round"], out int round);
            var path = $"https://api.currencyapi.com/v3/status";

            _httpClient.DefaultRequestHeaders.Clear();
            var message = new HttpRequestMessage();
            message.Headers.Add("apikey", apiKey);
            message.RequestUri = new Uri(path);
            message.Method = HttpMethod.Get;

            HttpResponseMessage apiResponse = apiResponse = await _httpClient.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JObject.Parse(apiContent);
            var currencySettings = new CurrencySettings();
            if (dataApiContent != null)
            {
                currencySettings.defaultCurrency = defaultCurrencyCode;
                currencySettings.baseCurrency = baseCurrencyCode;
                int.TryParse(Convert.ToString(dataApiContent["quotas"]["month"]["total"]), out int limit);
                int.TryParse(Convert.ToString(dataApiContent["quotas"]["month"]["used"]), out int used);
                currencySettings.requestLimit = limit;
                currencySettings.requestCount = used;
                currencySettings.currencyRoundCount = round;

                return currencySettings;
            }
            else
                throw new Exception();
        }
    }
}
