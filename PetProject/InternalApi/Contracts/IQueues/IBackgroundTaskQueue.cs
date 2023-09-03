using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues
{
    /// <summary>
    /// Интерфейс очереди фоновой задачи  
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Добавить задачу в очередь
        /// </summary>
        /// <param name="command">Задача</param>
        /// <returns>Операция записи</returns>
        ValueTask QueueAsync(WorkItem command);

        /// <summary>
        /// Получить задачу из очереди
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Операция чтения</returns>
        ValueTask<WorkItem> DequeueAsync(CancellationToken cancellationToken);   
    }

    /// <summary>
    /// Класс представляющий задачу
    /// </summary>
    public class WorkItem
    {
        /// <summary>
        /// Конструктор класс представляющего задачу
        /// </summary>
        /// <param name="id">идентификатор задачи</param>
        /// <param name="status">Статус задачи</param>
        public WorkItem(Guid id, CacheTackStatusDTO status)
        {
            Id = id;
            Status = status;
        }

        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Статус задачи
        /// </summary>
        public CacheTackStatusDTO Status { get; set; }
    }
}
