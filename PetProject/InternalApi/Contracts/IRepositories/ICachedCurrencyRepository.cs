using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;
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
        Task<CurrenciesDTO> GetCurrenciesOnDateFromDbAsync(DateOnly date, CancellationToken cancellationToken);

        /// <summary>
        /// Получает все записи курсов валют
        /// </summary>
        /// <param name="newBaseCurrency">Новая базовая валюта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список всех записей курсов валют</returns>
        Task<List<CurrenciesDTO>> GetAllCurrenciesFromDbAsync(string newBaseCurrency, CancellationToken cancellationToken);

        /// <summary>
        /// Добавляет задачу для пересчета кэша относительно новой базой валюты 
        /// </summary>
        /// <param name="newBaseCurrency">Базовая валюта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Объект состояния задачи</returns>
        Task<CacheTaskDTO> AddNewBaseCurrencyToCacheTaskAsync(string newBaseCurrency, CancellationToken cancellationToken);

        /// <summary>
        /// Получает задачу по уникальному индентификатору <paramref name="id"/>
        /// </summary>
        /// <param name="id">Уникальный индентификатор</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Объект состояния задачи</returns>
        Task<CacheTaskDTO> GetTaskFromCacheTaskAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Изменяет статус(состояние) задачи
        /// </summary>
        /// <param name="cacheTaskDTO">Объект состояния задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Изменение статуса(состояния задачи)</returns>
        Task ChangeStatusTaskToCacheTaskAsync(CacheTaskDTO cacheTaskDTO, CancellationToken cancellationToken);

        /// <summary>
        /// Сохраняет пересчитанный кэш в БД
        /// </summary>
        /// <param name="currencies">Список курсов валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сохранение результата пересчёта</returns>
        Task SaveNewCacheCurrenciesAsync(List<CurrenciesDTO> currencies, CancellationToken cancellationToken);

        /// <summary>
        /// Получает не завершенные задачи
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список не завершенных задач</returns>
        Task<List<CacheTaskDTO>> GetUnfinishedTasksAsync(CancellationToken cancellationToken);
    }
}