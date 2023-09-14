using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts
{
    /// <summary>
    /// Интерфейс пересчёта кэша
    /// </summary>
    public interface IRecalculateCacheService
    {
        /// <summary>
        /// Пересчитывает кэш
        /// </summary>
        /// <param name="workItem">Задача</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Пересчет кэша</returns>
        Task RecalculateCacheAsync(WorkItem workItem, CancellationToken cancellationToken);
    }
}
