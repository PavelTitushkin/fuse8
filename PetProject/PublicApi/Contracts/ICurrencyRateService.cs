using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;
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

        Task<FavoriteCurrencyDTO> GetFavoriteCurrencyAsync(string currencyName, CancellationToken cancellationToken);
        Task<List<FavoriteCurrencyDTO>> GetAllFavoritesCurrenciesAsync(CancellationToken cancellationToken);
        Task AddNewFavoriteCurrencyAsync(string currencyName, string currency, string currencyBase,  CancellationToken cancellationToken);
        Task ChangeFavoriteCurrencyByNameAsync(string currencyName, string changedCurrencyName, string changedCurrency, string changedCurrencyBase, CancellationToken cancellationToken);
        Task DeleteFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken);
    }
}
