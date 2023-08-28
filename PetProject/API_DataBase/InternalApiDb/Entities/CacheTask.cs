namespace DataStore.InternalApiDb.Entities
{
    public class CacheTask
    {
        public int Id { get; set; }
        public CacheStatus CacheStatus { get; set; }
        public Guid CacheStatusId { get; set; }
    }

    public class CacheStatus
    {
        public Guid Id { get; set; }
        public CacheTackStatus CacheTackStatus { get; set; }
    }

    public enum CacheTackStatus
    {
        Created,
        InProcessing,
        CompletedSuccessfully,
        CompletedWithError,
        Canceled
    }
}
