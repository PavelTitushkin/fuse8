using Fuse8_ByteMinds.SummerSchool.PublicApi.Models;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions
{
    public interface ICurrencyRateService
    {
        Task<HttpResponseMessage> GetCurrencyAsync();
        Task<HttpResponseMessage> GetCurrencyAsync(string currencyCode);
        Task<HttpResponseMessage> GetCurrencyAsync(string currencyCode, DateTime date);
        Task<HttpResponseMessage> GetCurrencySettingsAsync();
    }
}
