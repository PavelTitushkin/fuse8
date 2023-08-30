using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts
{
    public interface IRecalculateCacheService
    {
        Task RecalculateCacheAsync(WorkItem workItem, CancellationToken cancellationToken);
    }
}
