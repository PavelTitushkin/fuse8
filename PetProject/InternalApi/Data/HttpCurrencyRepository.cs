using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using InternalApi.Exceptions;
using InternalApi.Models.ModelResponse;
using InternalApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace InternalApi.Data
{
    public class HttpCurrencyRepository : ICurrencyRepository
    {
        private readonly HttpClient _httpClient;
        public AppSettings AppSettings { get; }

        public HttpCurrencyRepository(HttpClient httpClient, IOptions<AppSettings> options)
        {
            _httpClient = httpClient;
            AppSettings = options.Value;
            _httpClient.BaseAddress = new Uri(AppSettings.BasePath);
        }

        public async Task<CurrencyRateResponse> GetCurrencyRateAsync(CancellationToken cancellationToken)
        {
            var path = new Uri(_httpClient.BaseAddress + $"/latest?currencies={AppSettings.Default}&base_currency={AppSettings.Base}");
            AddDefaultRequestHeaders(_httpClient, AppSettings.APIKey);
            HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
            apiResponse.EnsureSuccessStatusCode();
            var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
        }
        public async Task<CurrencyRateResponse> GetCurrencyRateAsync(string currencyCode, CancellationToken cancellationToken)
        {
            try
            {
                var path = new Uri(_httpClient.BaseAddress + "/latest?currencies=" + currencyCode + "&base_currency=" + AppSettings.Base);
                AddDefaultRequestHeaders(_httpClient, AppSettings.APIKey);
                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
                apiResponse.EnsureSuccessStatusCode();
                var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);

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
        public async Task<CurrencyRateResponse> GetCurrencyOnDateRateAsync(string currencyCode, DateTime date, CancellationToken cancellationToken)
        {
            try
            {
                var dateString = date.ToString("yyyy-MM-dd");
                var path = new Uri(_httpClient.BaseAddress + $"/historical?currencies={currencyCode}&date={dateString}&base_currency={AppSettings.Base}");
                AddDefaultRequestHeaders(_httpClient, AppSettings.APIKey);
                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
                apiResponse.EnsureSuccessStatusCode();
                var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);

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
        public async Task<SettingsResponse> GetCurrencySettingsAsync(CancellationToken cancellationToken)
        {
            var path = new Uri(_httpClient.BaseAddress + "/status");
            AddDefaultRequestHeaders(_httpClient, AppSettings.APIKey);
            HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
            apiResponse.EnsureSuccessStatusCode();
            var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<SettingsResponse>(apiContent);
        }


        public async Task<CurrencyRateResponse> GetCurrenciesRateAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            try
            {
                var path = new Uri(_httpClient.BaseAddress + $"/latest?base_currency={baseCurrency}");
                AddDefaultRequestHeaders(_httpClient, AppSettings.APIKey);

                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
                apiResponse.EnsureSuccessStatusCode();

                var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);

                return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);

            }
            catch (HttpRequestException ex)
            {
                if ((int)ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                {
                    throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task<CurrencyRateResponse> GetCurrenciesOnDateRateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            try
            {
                var dateString = date.ToString("yyyy-MM-dd");
                var path = new Uri(_httpClient.BaseAddress + $"/historical?date={dateString}&base_currency={baseCurrency}");
                AddDefaultRequestHeaders(_httpClient, AppSettings.APIKey);

                HttpResponseMessage apiResponse = await _httpClient.GetAsync(path, cancellationToken);
                apiResponse.EnsureSuccessStatusCode();
                var apiContent = await apiResponse.Content.ReadAsStringAsync(cancellationToken);

                return JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            }
            catch (HttpRequestException ex)
            {
                if ((int)ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                {
                    throw new CurrencyNotFoundException("Запрос к несуществующей валюте.");
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static void AddDefaultRequestHeaders(HttpClient httpClient, string apiKey)
        {
            if (httpClient.DefaultRequestHeaders.Contains("apikey") is false)
                httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
        }
    }
}
