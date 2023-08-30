namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO
{
    public class CacheTaskDTO
    {
        public Guid Id { get; set; }
        public CacheTackStatusDTO CacheTackStatus { get; set; }
        public string NewBaseCurrency { get; set; }
        public DateTime Created { get; set; }
    }

    public enum CacheTackStatusDTO
    {
        Created,
        InProcessing,
        CompletedSuccessfully,
        CompletedWithError,
        Canceled
    }
}
