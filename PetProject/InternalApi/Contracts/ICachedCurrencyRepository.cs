using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts
{
    public interface ICachedCurrencyRepository
    {
        string FindCacheFile();
        string FindCacheFileOnDate(DateOnly date);
        Task WriteCurrenciesToCacheFileAsync(Currency[] currencies, CancellationToken cancellationToken);
        Task WriteCurrenciesOnDateToCacheFileAsync(Currency[] currencies, DateOnly date, CancellationToken cancellationToken);
        Task<Currency[]> GetCurrencyFromCacheFile(string cacheFile, CancellationToken cancellationToken);
    }
}