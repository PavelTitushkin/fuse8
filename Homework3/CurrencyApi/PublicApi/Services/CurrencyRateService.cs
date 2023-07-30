using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using System.Text;
using System.Text.Json;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    /// <summary>
    /// Сервис для работы с currencyApi
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public CurrencyRateService(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
        }

        public async Task<HttpResponseMessage> GetCurrencyAsync()
        {
            if (GetCurrencyLimitAsync().Result)
            {
                var apiKey = _appSettings.APIKey;
                var defaultCurrencyCode = _appSettings.Default;
                var baseCurrencyCode = _appSettings.Base;
                var path = new StringBuilder(_appSettings.BasePath);
                var fullPath = path.Append("/latest?currencies=").Append(defaultCurrencyCode).Append("&base_currency=").Append(baseCurrencyCode).ToString();

                if (_httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                    _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

                HttpResponseMessage apiResponse = await _httpClient.GetAsync(fullPath);

                return apiResponse.EnsureSuccessStatusCode();
            }
            else
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }

        public async Task<HttpResponseMessage> GetCurrencyAsync(string currencyCode)
        {
            if (GetCurrencyLimitAsync().Result)
            {
                var apiKey = _appSettings.APIKey;
                var baseCurrencyCode = _appSettings.Base;
                var path = new StringBuilder(_appSettings.BasePath);
                var fullPath = path.Append("/latest?currencies=").Append(currencyCode).Append("&base_currency=").Append(baseCurrencyCode).ToString();

                if (_httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                    _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

                HttpResponseMessage apiResponse = await _httpClient.GetAsync(fullPath);

                if ((int)apiResponse.StatusCode == StatusCodes.Status422UnprocessableEntity)
                    throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");

                return apiResponse.EnsureSuccessStatusCode();
            }
            else
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }

        public async Task<HttpResponseMessage> GetCurrencyAsync(string currencyCode, DateTime date)
        {
            if (GetCurrencyLimitAsync().Result)
            {
                var dateString = date.ToString("yyyy-MM-dd");
                var apiKey = _appSettings.APIKey;
                var baseCurrencyCode = _appSettings.Base;
                var path = new StringBuilder(_appSettings.BasePath);
                var fullPath = path.Append("/historical?currencies=").Append(currencyCode).Append("&date=").Append(dateString).Append("&base_currency=").Append(baseCurrencyCode).ToString();

                if (_httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                    _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

                HttpResponseMessage apiResponse = await _httpClient.GetAsync(fullPath);

                if (apiResponse.StatusCode.ToString() == "UnprocessableEntity")
                    throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");

                return apiResponse.EnsureSuccessStatusCode();
            }
            else
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }

        public async Task<HttpResponseMessage> GetCurrencySettingsAsync()
        {
            var apiKey = _appSettings.APIKey;
            var path = new StringBuilder(_appSettings.BasePath);
            var fullPath = path.Append("/status").ToString();

            if (_httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(fullPath);

            return apiResponse.EnsureSuccessStatusCode();
        }

        private async Task<bool> GetCurrencyLimitAsync()
        {
            var apiKey = _appSettings.APIKey;
            var path = new StringBuilder(_appSettings.BasePath);
            var fullPath = path.Append("/status").ToString();

            if (_httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(fullPath);
            apiResponse.EnsureSuccessStatusCode();

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<SettingsResponse>(apiContent);

            if (dataApiContent.Quotas.Month.Total > 0)
                return true;
            else
                return false;
        }
    }
}
