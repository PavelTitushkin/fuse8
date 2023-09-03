using InternalApi.Models.ModelResponse;

namespace InternalApi.Contracts
{
    /// <summary>
    /// Интерфейс для работы с CurrencyRateService
    /// </summary>
    public interface ICurrencyRateService : ICurrencyAPI
    {
        /// <summary>
        /// Получает курс валюты по умолчанию относительно базовой
        /// </summary>
        /// <returns>Курс валюты.</returns>
        Task<Currency> GetCurrencyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получает курс <paramref name="currencyCode"/> относительно базовой. 
        /// </summary>
        /// <param name="currencyCode">Код валюты</param>
        /// <returns>Курс валюты</returns>
        Task<Currency> GetCurrencyAsync(string currencyCode, CancellationToken cancellationToken);

        /// <summary>
        /// Получает курс <paramref name="currencyCode"/> относительно базовой на определённую <paramref name="date"/> 
        /// </summary>
        /// <param name="currencyCode">Код валюты</param>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <returns>Курс валюты на определённую дату</returns>
        Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date, CancellationToken cancellationToken);

        /// <summary>
        /// Получает настройки API
        /// </summary>
        /// <returns>Настройки API</returns>
        Task<CurrencySettings> GetCurrencySettingsAsync(CancellationToken cancellationToken);
    }
}
