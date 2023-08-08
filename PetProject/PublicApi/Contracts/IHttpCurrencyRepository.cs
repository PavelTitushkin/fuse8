using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;

namespace PublicApi.Contracts
{
    public interface IHttpCurrencyRepository
    {
        Task<CurrencyRateResponse> GetCurrencyRateAsync();
        Task<CurrencyRateResponse> GetCurrencyRateAsync(string currencyCode);
        Task<CurrencyRateResponse> GetCurrencyOnDateRateAsync(string currencyCode, DateTime date);
        Task<SettingsResponse> GetCurrencySettingsAsync();

        Task<CurrencyRateResponse> GetCurrenciesRateAsync(string baseCurrency, CancellationToken cancellationToken);
        Task<CurrencyRateResponse> GetCurrenciesOnDateRateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken);
    }
}