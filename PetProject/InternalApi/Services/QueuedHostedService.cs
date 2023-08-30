using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Services
{
    public class QueuedHostedService : BackgroundService
    {
        private IServiceProvider _services;
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        public QueuedHostedService(IServiceProvider services, ILogger<QueuedHostedService> logger, IBackgroundTaskQueue taskQueue)
        {
            _services = services;
            _logger = logger;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                await DoUnfinishedTasks(stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    using var scope = _services.CreateScope();
                    var worker = scope.ServiceProvider.GetRequiredService<IRecalculateCacheService>();
                    await worker.RecalculateCacheAsync(workItem, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка во время обработки {id} {status}", workItem.Id, workItem.Status);
                }
            }
        }

        private async Task DoUnfinishedTasks(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICachedCurrencyRepository>();

            var task = await repository.GetUnfinishedTasksAsync(stoppingToken);
            if (task.Count > 1)
            {
                task.Sort((t1, t2) => t2.Created.CompareTo(t1.Created));
                var unfinisedTask = new WorkItem(task[0].Id, task[0].CacheTackStatus);
                await _taskQueue.QueueAsync(unfinisedTask);
                for (int i = 1; i < task.Count; i++)
                {
                    task[i].CacheTackStatus = CacheTackStatusDTO.Canceled;
                    task[i].Created = DateTime.UtcNow;
                    await repository.ChangeStatusTaskToCacheTaskAsync(task[i], stoppingToken);
                }
            }
            if (task.Count == 1)
            {
                var unfinisedTask = new WorkItem(task[0].Id, task[0].CacheTackStatus);
                await _taskQueue.QueueAsync(unfinisedTask);
            }
        }
    }
}
