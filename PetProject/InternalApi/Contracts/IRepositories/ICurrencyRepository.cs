using InternalApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories
{
    /// <summary>
    /// Интерфейс для работы с репозиторием
    /// </summary>
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Получает курсы валют по умолчанию относительно базовой
        /// </summary>
        /// <returns>Ответ внешнего Api</returns>
        Task<CurrencyRateResponse> GetCurrencyRateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получает курсы валют <paramref name="currencyCode"/> относительно базовой. 
        /// </summary>
        /// <param name="currencyCode">Код валюты</param>
        /// <returns>Ответ внешнего Api</returns>
        Task<CurrencyRateResponse> GetCurrencyRateAsync(string currencyCode, CancellationToken cancellationToken);

        /// <summary>
        /// Получает курс валют <paramref name="currencyCode"/> относительно базовой на определённую <paramref name="date"/> 
        /// </summary>
        /// <param name="currencyCode">Код валюты</param>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <returns>Ответ внешнего Api</returns>
        Task<CurrencyRateResponse> GetCurrencyOnDateRateAsync(string currencyCode, DateTime date, CancellationToken cancellationToken);

        /// <summary>
        /// Получает настройки внешнего Api
        /// </summary>
        /// <returns>Ответ внешнего Api</returns>
        Task<SettingsResponse> GetCurrencySettingsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получает текущий курс для всех валют
        /// </summary>
        /// <param name="baseCurrency">Базовая валюта, относительно которой необходимо получить курс</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ внешнего Api</returns>
        Task<CurrencyRateResponse> GetCurrenciesRateAsync(string baseCurrency, CancellationToken cancellationToken);

        /// <summary>
        /// Получает курс для всех валют, актуальный на <paramref name="date"/>
        /// </summary>
        /// <param name="baseCurrency">Базовая валюта, относительно которой необходимо получить курс</param>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ внешнего Api</returns>
        Task<CurrencyRateResponse> GetCurrenciesOnDateRateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken);
    }
}