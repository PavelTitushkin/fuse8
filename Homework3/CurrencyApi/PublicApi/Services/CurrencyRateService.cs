using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
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
            _httpClient.BaseAddress = new Uri(appSettings.BasePath);
        }

        public async Task<Currency> GetCurrencyAsync()
        {
            if (await GetCurrencyLimitAsync())
            {
                try
                {
                    var apiKey = _appSettings.APIKey;
                    var defaultCurrencyCode = _appSettings.Default;
                    var baseCurrencyCode = _appSettings.Base;
                    var round = _appSettings.Round;
                    var path = new Uri(_httpClient.BaseAddress + "/latest?currencies=" + defaultCurrencyCode + "&base_currency=" + baseCurrencyCode);
                    AddDefaultRequestHeaders(_httpClient, apiKey);
                    HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
                    apiResponse.EnsureSuccessStatusCode();
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
                    var currency = new Currency();
                    var data = dataApiContent.Data[defaultCurrencyCode];
                    currency.Code = data.Code;
                    currency.Value = Math.Round(data.Value, round);

                    return currency;
                }
                catch (HttpRequestException ex)
                {
                    throw ex;
                }
            }
            else
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }

        public async Task<Currency> GetCurrencyAsync(string currencyCode)
        {
            if (await GetCurrencyLimitAsync())
            {
                try
                {
                    var apiKey = _appSettings.APIKey;
                    var baseCurrencyCode = _appSettings.Base;
                    var round = _appSettings.Round;
                    var path = new Uri(_httpClient.BaseAddress + "/latest?currencies=" + currencyCode + "&base_currency=" + baseCurrencyCode);
                    AddDefaultRequestHeaders(_httpClient, apiKey);
                    HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
                    apiResponse.EnsureSuccessStatusCode();
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
                    var currency = new Currency();
                    var data = dataApiContent.Data[currencyCode];
                    currency.Code = data.Code;
                    currency.Value = Math.Round(data.Value, round);

                    return currency;
                }
                catch (HttpRequestException ex)
                {
                    if ((int)ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                        throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");
                    else
                        throw ex;
                }
            }
            else
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }

        public async Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date)
        {
            if (await GetCurrencyLimitAsync())
            {
                try
                {
                    var dateString = date.ToString("yyyy-MM-dd");
                    var apiKey = _appSettings.APIKey;
                    var baseCurrencyCode = _appSettings.Base;
                    var round = _appSettings.Round;
                    var path = new Uri(_httpClient.BaseAddress + "/historical?currencies=" + currencyCode + "&date=" + dateString + "&base_currency=" + baseCurrencyCode);
                    AddDefaultRequestHeaders(_httpClient, apiKey);
                    HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
                    apiResponse.EnsureSuccessStatusCode();
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
                    var currencyWithDate = new CurrencyWithDate();
                    var data = dataApiContent.Data[currencyCode];
                    currencyWithDate.Date = dataApiContent.Meta.LastUpdatedAt.ToString("yyyy-MM-dd");
                    currencyWithDate.Code = data.Code;
                    currencyWithDate.Value = Math.Round(data.Value, round);

                    return currencyWithDate;
                }
                catch (HttpRequestException ex)
                {
                    if ((int)ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                        throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");
                    else
                        throw ex;
                }
            }
            else
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }

        public async Task<CurrencySettings> GetCurrencySettingsAsync()
        {
            try
            {
                var apiKey = _appSettings.APIKey;
                var defaultCurrencyCode = _appSettings.Default;
                var baseCurrencyCode = _appSettings.Base;
                var round = _appSettings.Round;
                var path = new Uri(_httpClient.BaseAddress + "/status");
                AddDefaultRequestHeaders(_httpClient, apiKey);
                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
                apiResponse.EnsureSuccessStatusCode();
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var dataApiContent = JsonSerializer.Deserialize<SettingsResponse>(apiContent);
                var currencySettings = new CurrencySettings();
                currencySettings.DefaultCurrency = defaultCurrencyCode;
                currencySettings.BaseCurrency = baseCurrencyCode;
                currencySettings.RequestLimit = dataApiContent.Quotas.Month.Total;
                currencySettings.RequestCount = dataApiContent.Quotas.Month.Used;
                currencySettings.CurrencyRoundCount = round;

                return currencySettings;
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
        }

        private async Task<bool> GetCurrencyLimitAsync()
        {
            var apiSettings = new CurrencySettings();
            apiSettings = await GetCurrencySettingsAsync();
            var remainingRequests = apiSettings.RequestLimit - apiSettings.RequestCount;
            if (remainingRequests > 0)
                return true;
            else
                return false;
        }

        private void AddDefaultRequestHeaders(HttpClient httpClient, string apiKey)
        {
            if (httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
        }
    }
}
