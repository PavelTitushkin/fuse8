using Fuse8_ByteMinds.SummerSchool.PublicApi.Models;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions
{
    public interface ICurrencyRateService
    {
        Task<Currency> GetCurrencyAsync();
        Task<Currency> GetCurrencyAsync(string currencyCode);
        Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date);
        Task<CurrencySettings> GetCurrencySettingsAsync();
    }
}
