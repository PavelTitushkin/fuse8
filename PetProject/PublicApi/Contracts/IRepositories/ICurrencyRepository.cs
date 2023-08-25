using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories
{
    public interface ICurrencyRepository
    {
        Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken);
        Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken);

        Task<FavoriteCurrencyDTO?> GetFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken);
        Task<List<FavoriteCurrencyDTO>> GetAllFavoritesCurrenciesAsync(CancellationToken cancellationToken);
        Task AddNewFavoriteCurrencyAsync(FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken);
        Task ChangeFavoriteCurrencyByNameAsync(string currencyName, FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken);
        Task DeleteFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken);
    }
}
