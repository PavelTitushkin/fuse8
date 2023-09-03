using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Services
{
    public class RecalculateCacheService : IRecalculateCacheService
    {
        private readonly ICachedCurrencyRepository _cachedCurrencyRepository;
        private readonly ILogger<RecalculateCacheService> _logger;

        public AppSettings AppSettings { get; set; }
        public RecalculateCacheService(IOptions<AppSettings> options, ICachedCurrencyRepository cachedCurrencyRepository, ILogger<RecalculateCacheService> logger)
        {
            _cachedCurrencyRepository = cachedCurrencyRepository;
            _logger = logger;
            AppSettings = options.Value;
        }

        public async Task RecalculateCacheAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            if (workItem.Status == CacheTackStatusDTO.Created)
            {
                await InProcessingAsync(workItem, cancellationToken);
            }
            if (workItem.Status == CacheTackStatusDTO.InProcessing)
            {
                await RecalculateAsync(workItem, cancellationToken);
            }
        }

        private async Task RecalculateAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            try
            {
                var task = await _cachedCurrencyRepository.GetTaskFromCacheTaskAsync(workItem.Id, cancellationToken);
                var newBaseCurrency = task.NewBaseCurrency;
                var cacheList = await _cachedCurrencyRepository.GetAllCurrenciesFromDbAsync(newBaseCurrency, cancellationToken);
                foreach (var cache in cacheList)
                {
                    var newBaseCurrencyValue = cache.CurrenciesList
                        .Where(c => c.Code == newBaseCurrency)
                        .FirstOrDefault().Value;
                    foreach (var item in cache.CurrenciesList)
                    {
                        item.Value = Math.Round(item.Value / newBaseCurrencyValue, AppSettings.Round);
                    }
                    cache.CurrenciesList.OrderBy(c => c.Code);
                }
                await _cachedCurrencyRepository.SaveNewCacheCurrenciesAsync(cacheList, cancellationToken);
                AppSettings.Base = newBaseCurrency;
                task.CacheTackStatus = CacheTackStatusDTO.CompletedSuccessfully;
                task.Created = DateTime.UtcNow;
                await _cachedCurrencyRepository.ChangeStatusTaskToCacheTaskAsync(task, cancellationToken);
                workItem.Status = CacheTackStatusDTO.CompletedSuccessfully;
            }
            catch (Exception ex)
            {
                var task = await _cachedCurrencyRepository.GetTaskFromCacheTaskAsync(workItem.Id, cancellationToken);
                task.CacheTackStatus = CacheTackStatusDTO.CompletedWithError;
                await _cachedCurrencyRepository.ChangeStatusTaskToCacheTaskAsync(task, cancellationToken);
                workItem.Status = CacheTackStatusDTO.CompletedWithError;
                _logger.LogError(ex, "Ошибка во время пересчёта кэша: {Id} {CacheTackStatus}", task.Id, task.CacheTackStatus);
            }
        }

        private async Task InProcessingAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var task = await _cachedCurrencyRepository.GetTaskFromCacheTaskAsync(workItem.Id, cancellationToken);
            task.CacheTackStatus = CacheTackStatusDTO.InProcessing;
            task.Created = DateTime.UtcNow;
            await _cachedCurrencyRepository.ChangeStatusTaskToCacheTaskAsync(task, cancellationToken);
            workItem.Status = CacheTackStatusDTO.InProcessing;            
        }
    }
}
