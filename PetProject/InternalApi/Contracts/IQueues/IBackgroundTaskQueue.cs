using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(WorkItem command);
        ValueTask<WorkItem> DequeueAsync(CancellationToken cancellationToken);   
    }

    public class WorkItem
    {
        public WorkItem(Guid id, CacheTackStatusDTO status)
        {
            Id = id;
            Status = status;
        }

        public Guid Id { get; set; }
        public CacheTackStatusDTO Status { get; set; }
    }
}
