using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions
{
    /// <summary>
    /// Интерфейс для работы с CurrencyRateService
    /// </summary>
    public interface ICurrencyRateService
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
        Task<FavoriteCurrencyDTO> GetFavoriteCurrencyAsync(string currencyName, CancellationToken cancellationToken);

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
        Task AddNewFavoriteCurrencyAsync(string currencyName, string currency, string currencyBase,  CancellationToken cancellationToken);

        /// <summary>
        /// Изменяет Избранную валюту <paramref name="currencyName"/> на <paramref name="currenciesDTO"/>
        /// </summary>
        /// <param name="currencyName">Название избранной валюты</param>
        /// <param name="currenciesDTO"><inheritdoc cref="FavoriteCurrencyDTO"/></param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат изменения</returns>
        Task ChangeFavoriteCurrencyByNameAsync(string currencyName, string changedCurrencyName, string changedCurrency, string changedCurrencyBase, CancellationToken cancellationToken);

        /// <summary>
        /// Удаляет Избранную валюту по <paramref name="currencyName"/>
        /// </summary>
        /// <param name="currencyName">Название избранной валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task DeleteFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken);
    }
}
