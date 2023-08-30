using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;
using System.Threading.Channels;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Queues
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<WorkItem> _queue;

        public BackgroundTaskQueue()
        {
            var options = new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.Wait };
            _queue = Channel.CreateBounded<WorkItem>(options);
        }

        public ValueTask QueueAsync(WorkItem command)
        {
           return _queue.Writer.WriteAsync(command);
        }

        public ValueTask<WorkItem> DequeueAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
