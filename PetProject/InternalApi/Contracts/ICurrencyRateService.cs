using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions
{
    public interface ICurrencyRateService
    {
        Task<Currency> GetCurrencyAsync();
        Task<Currency> GetCurrencyAsync(string currencyCode);
        Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date);
        Task<CurrencySettings> GetCurrencySettingsAsync();
    }
}
