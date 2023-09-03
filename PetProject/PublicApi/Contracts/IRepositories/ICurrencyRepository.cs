using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories
{
    /// <summary>
    /// Интерфейс для работы с репозиториями
    /// </summary>
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Изменяет <paramref name="defaultCurrency"/>
        /// </summary>
        /// <param name="defaultCurrency">Код валюты по умолчанию</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Изменение кода валюты по умрлчанию</returns>
        Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken);

        /// <summary>
        /// Изменяет <paramref name="round"/>
        /// </summary>
        /// <param name="round">Количество знаков после запятой</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Изменение количества знаков после запятой</returns>
        Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken);

        /// <summary>
        /// Получает Избранную валюту по <paramref name="currencyName"/>
        /// </summary>
        /// <param name="currencyName">Код Избранной валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns><inheritdoc cref="FavoriteCurrencyDTO"/></returns>
        Task<FavoriteCurrencyDTO?> GetFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken);

        /// <summary>
        /// Получает список всех Избранных валют
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список <inheritdoc cref="FavoriteCurrencyDTO"/></returns>
        Task<List<FavoriteCurrencyDTO>> GetAllFavoritesCurrenciesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Добавляет новую Избранную валюту
        /// </summary>
        /// <param name="currenciesDTO"><inheritdoc cref="FavoriteCurrencyDTO"/></param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат добавления</returns>
        Task AddNewFavoriteCurrencyAsync(FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Изменяет Избранную валюту <paramref name="currencyName"/> на <paramref name="currenciesDTO"/>
        /// </summary>
        /// <param name="currencyName">Название избранной валюты</param>
        /// <param name="currenciesDTO"><inheritdoc cref="FavoriteCurrencyDTO"/></param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат изменения</returns>
        Task ChangeFavoriteCurrencyByNameAsync(string currencyName, FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Удаляет Избранную валюту по <paramref name="currencyName"/>
        /// </summary>
        /// <param name="currencyName">Название избранной валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task DeleteFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken);
    }
}
