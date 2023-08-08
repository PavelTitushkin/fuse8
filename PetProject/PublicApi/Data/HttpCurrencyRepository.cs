using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;
using PublicApi.Contracts;
using System.Text.Json;

namespace InternalApi.Data
{
    public class HttpCurrencyRepository : IHttpCurrencyRepository
    {
        private readonly HttpClient _httpClient;
        public AppSettings AppSettings { get; }

        public HttpCurrencyRepository(HttpClient httpClient, IOptions<AppSettings> options)
        {
            _httpClient = httpClient;
            AppSettings = options.Value;
            _httpClient.BaseAddress = new Uri(AppSettings.BasePath);
        }

        public async Task<CurrencyRateResponse> GetCurrencyRateAsync()
        {
            var apiKey = AppSettings.APIKey;
            var defaultCurrencyCode = AppSettings.Default;
            var baseCurrencyCode = AppSettings.Base;
            var path = new Uri(_httpClient.BaseAddress + $"/latest?currencies={defaultCurrencyCode}&base_currency={baseCurrencyCode}");
            AddDefaultRequestHeaders(_httpClient, apiKey);
            HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
            apiResponse.EnsureSuccessStatusCode();
            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
        }
        public async Task<CurrencyRateResponse> GetCurrencyRateAsync(string currencyCode)
        {
            try
            {
                var apiKey = AppSettings.APIKey;
                var baseCurrencyCode = AppSettings.Base;
                var path = new Uri(_httpClient.BaseAddress + "/latest?currencies=" + currencyCode + "&base_currency=" + baseCurrencyCode);
                AddDefaultRequestHeaders(_httpClient, apiKey);
                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
                apiResponse.EnsureSuccessStatusCode();
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            }
            catch (HttpRequestException ex)
            {
                if ((int)ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                    throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");
                else
                    throw ex;
            }
        }

        public async Task<CurrencyRateResponse> GetCurrencyOnDateRateAsync(string currencyCode, DateTime date)
        {
            try
            {
                var dateString = date.ToString("yyyy-MM-dd");
                var apiKey = AppSettings.APIKey;
                var baseCurrencyCode = AppSettings.Base;
                var path = new Uri(_httpClient.BaseAddress + $"/historical?currencies={currencyCode}&date={dateString}&base_currency={baseCurrencyCode}");
                AddDefaultRequestHeaders(_httpClient, apiKey);
                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
                apiResponse.EnsureSuccessStatusCode();
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            }
            catch (HttpRequestException ex)
            {
                if ((int)ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                    throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");
                else
                    throw ex;
            }
        }

        public async Task<SettingsResponse> GetCurrencySettingsAsync()
        {
            var apiKey = AppSettings.APIKey;
            var path = new Uri(_httpClient.BaseAddress + "/status");
            AddDefaultRequestHeaders(_httpClient, apiKey);
            HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
            apiResponse.EnsureSuccessStatusCode();
            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<SettingsResponse>(apiContent);
        }



        public async Task<CurrencyRateResponse> GetCurrenciesRateAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            var apiKey = AppSettings.APIKey;
            var path = new Uri(_httpClient.BaseAddress + $"/latest?base_currency={baseCurrency}");
            AddDefaultRequestHeaders(_httpClient, apiKey);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
            apiResponse.EnsureSuccessStatusCode();

            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
        }

        public async Task<CurrencyRateResponse> GetCurrenciesOnDateRateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            var apiKey = AppSettings.APIKey;
            var dateString = date.ToString("yyyy-MM-dd");
            var path = new Uri(_httpClient.BaseAddress + $"/historical?date={dateString}&base_currency={baseCurrency}");
            AddDefaultRequestHeaders(_httpClient, apiKey);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(path);
            apiResponse.EnsureSuccessStatusCode();
            var apiContent = await apiResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
        }

        private void AddDefaultRequestHeaders(HttpClient httpClient, string apiKey)
        {
            if (httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
        }
    }
}
