using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories
{
    /// <summary>
    /// Интерфейс для работы CachedCurrencyRepository
    /// </summary>
    public interface ICachedCurrencyRepository
    {
        /// <summary>
        /// Получает список валют
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        Task<Currency[]> GetCurrentCurrenciesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получает список валют относительно <paramref name="date"/>
        /// </summary>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        Task<Currency[]> GetCurrenciesOnDateAsync(DateOnly date, CancellationToken cancellationToken);

        /// <summary>
        /// Получает список валют
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        Task<CurrenciesDTO> GetCurrentCurrenciesFromDbAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получает список валют относительно <paramref name="date"/>
        /// </summary>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        Task<Currency[]> GetCurrenciesOnDateFromDbAsync(DateOnly date, CancellationToken cancellationToken);

    }
}