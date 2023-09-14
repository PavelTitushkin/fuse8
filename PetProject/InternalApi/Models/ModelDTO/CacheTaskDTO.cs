namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO
{
    /// <summary>
    /// Класс задачи 
    /// </summary>
    public class CacheTaskDTO
    {
        /// <summary>
        /// Уникальный индентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// <inheritdoc cref="CacheTackStatusDTO"/>
        /// </summary>
        public CacheTackStatusDTO CacheTackStatus { get; set; }

        /// <summary>
        /// Значение новой базовой валюты
        /// </summary>
        public string NewBaseCurrency { get; set; }

        /// <summary>
        /// Время изменения статуса
        /// </summary>
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// Статус задачи
    /// </summary>
    public enum CacheTackStatusDTO
    {
        Created,
        InProcessing,
        CompletedSuccessfully,
        CompletedWithError,
        Canceled
    }
}
