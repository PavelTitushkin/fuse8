namespace DataInternalApi.Entities
{
    public class CacheTask
    {
        public Guid Id { get; set; }
        public CacheTackStatus CacheTackStatus { get; set; }
        public string NewBaseCurrency { get; set; }
        public DateTime Created { get; set; }
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
