using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions
{
    public interface ICurrencyRateService
    {
        Task<Currency> GetCurrencyAsync();
        Task<Currency> GetCurrencyAsync(string currencyCode);
        Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date);
        Task<CurrencySettings> GetCurrencySettingsAsync();

        Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken);
        Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken);


    }
}
