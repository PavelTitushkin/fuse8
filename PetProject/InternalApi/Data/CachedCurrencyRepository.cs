using DataStore.InternalApiDb;
using DataStore.InternalApiDb.Entities;
using AutoMapper;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;
using InternalApi.Contracts;
using InternalApi.Models.ModelResponse;
using InternalApi.Models.ModelsConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace InternalApi.Data
{
    public class CachedCurrencyRepository : ICachedCurrencyRepository
    {
        private readonly ICurrencyAPI _currencyAPI;
        private readonly InternalApiContext _internalApiContext;
        private readonly IMapper _mapper;
        public AppSettings AppSettings { get; }


        public CachedCurrencyRepository(IOptions<AppSettings> options, ICurrencyAPI currencyAPI,
            InternalApiContext currencyRateContext,
            IMapper mapper)
        {
            AppSettings = options.Value;
            _currencyAPI = currencyAPI;
            _internalApiContext = currencyRateContext;
            _mapper = mapper;
        }

        public async Task<Currency[]> GetCurrentCurrenciesAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cachedCurrencies = await GetCachedCurrencies(cancellationToken);
            if (cachedCurrencies == null)
            {
                var currency = AppSettings.Base;
                var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(currency, cancellationToken);
                await WriteCurrenciesToCacheFileAsync(currencies, cancellationToken);

                return currencies;
            }

            return cachedCurrencies;
        }

        public async Task<Currency[]> GetCurrenciesOnDateAsync(DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cachedCurrencies = await GetCachedCurrenciesOnDate(date, cancellationToken);
            if (cachedCurrencies == null)
            {
                var currency = AppSettings.Base;
                var currenciesOnDate = await _currencyAPI.GetAllCurrenciesOnDateAsync(currency, date, cancellationToken);
                var currencies = currenciesOnDate.Currencies;
                await WriteCurrenciesOnDateToCacheFileAsync(currencies, date, cancellationToken);

                return currencies;
            }

            return cachedCurrencies;
        }

        public async Task<CurrenciesDTO> GetCurrentCurrenciesFromDbAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var dateNow = DateTime.UtcNow;
            var cachedCurrencies = await _internalApiContext.CurrenciesList
                .Include(c => c.CurrenciesList)
                .Where(x => (dateNow - x.Date).TotalHours < AppSettings.CacheLifetime)
                .Select(currencies => _mapper.Map<CurrenciesDTO>(currencies))
                .FirstOrDefaultAsync(cancellationToken);

            if (cachedCurrencies == null)
            {
                var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(AppSettings.Base, cancellationToken);
                var currenciesDto = new CurrenciesDTO
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    CurrenciesList = currencies.ToList()
                };

                var currenciesToDb = _mapper.Map<Currencies>(currenciesDto);

                await _internalApiContext.CurrenciesList.AddAsync(currenciesToDb, cancellationToken);
                await _internalApiContext.SaveChangesAsync(cancellationToken);

                return currenciesDto;
            }

            return cachedCurrencies;
        }

        public async Task<CurrenciesDTO> GetCurrenciesOnDateFromDbAsync(DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var dateTime = date.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
            var cachedCurrencies = await _internalApiContext.CurrenciesList
                .Include(c => c.CurrenciesList)
                .Where(c => c.Date == dateTime)
                .Select(currencies => _mapper.Map<CurrenciesDTO>(currencies))
                .FirstOrDefaultAsync(cancellationToken);

            if (cachedCurrencies == null)
            {
                var currenciesOnDate = await _currencyAPI.GetAllCurrenciesOnDateAsync(AppSettings.Base, date, cancellationToken);
                var currencies = currenciesOnDate.Currencies;

                var currenciesDto = new CurrenciesDTO
                {
                    Id = 0,
                    Date = dateTime,
                    CurrenciesList = currencies.ToList()
                };

                var currenciesToDb = _mapper.Map<Currencies>(currenciesDto);

                await _internalApiContext.CurrenciesList.AddAsync(currenciesToDb, cancellationToken);
                await _internalApiContext.SaveChangesAsync(cancellationToken);

                return currenciesDto;
            }

            return cachedCurrencies;
        }

        public async Task<List<CurrenciesDTO>> GetAllCurrenciesFromDbAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cachedCurrencies = await _internalApiContext.CurrenciesList
                .AsNoTracking()
                .Include(c => c.CurrenciesList)
                .OrderBy(c => c.Date)
                .Select(e => _mapper.Map<CurrenciesDTO>(e))
                .ToListAsync(cancellationToken);

            if (cachedCurrencies.Count == 0)
            {
                var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(AppSettings.Base, cancellationToken);
                var currenciesDto = new CurrenciesDTO
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    CurrenciesList = currencies.ToList()
                };

                return new List<CurrenciesDTO>
                {
                    currenciesDto
                };
            }
            return cachedCurrencies;
        }


        //Запись Currency[] в кэш-файл
        private async Task WriteCurrenciesToCacheFileAsync(Currency[] currencies, CancellationToken cancellationToken)
        {
            var path = Path.Combine(AppSettings.PathFile, DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm") + ".json");
            await using FileStream createStream = File.Create(path);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            await JsonSerializer.SerializeAsync(createStream, currencies, options, cancellationToken);
            await createStream.DisposeAsync();
        }

        //Запись Currency[] в кэш-файл
        private async Task WriteCurrenciesOnDateToCacheFileAsync(Currency[] currencies, DateOnly date, CancellationToken cancellationToken)
        {
            var path = Path.Combine(AppSettings.PathFile, date.ToString("yyyy-MM-dd-HH-mm") + ".json");
            await using FileStream createStream = File.Create(path);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            await JsonSerializer.SerializeAsync(createStream, currencies, options, cancellationToken);
            await createStream.DisposeAsync();
        }

        //Получение Currency из кэш-файла
        private async Task<Currency[]> GetCurrencyFromCacheFile(string cacheFile, CancellationToken cancellationToken)
        {
            var pathFile = AppSettings.PathFile;
            var filePathJson = Path.Combine(pathFile, $"{cacheFile}.json");
            var currensiesText = await File.ReadAllTextAsync(filePathJson, cancellationToken);
            var currencies = JsonSerializer.Deserialize<Currency[]>(currensiesText);

            return currencies;
        }

        //Поиск кэш-файла
        private async Task<Currency[]> GetCachedCurrencies(CancellationToken cancellationToken)
        {
            var pathFile = AppSettings.PathFile;
            var dateNow = DateTime.UtcNow;
            var search = ".json";
            var cacheLifetime = AppSettings.CacheLifetime;
            var file = Directory.EnumerateFiles(pathFile, "*.json");
            var cacheDictionary = new Dictionary<string, double>();
            foreach (var item in file)
            {
                var fileName = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
                var fileNameToDate = DateTime.ParseExact(fileName, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
                cacheDictionary[fileName] = (dateNow - fileNameToDate).TotalHours;
            }

            var targetFile = cacheDictionary.OrderBy(x => x.Value).FirstOrDefault(x => x.Value < cacheLifetime).Key;

            return targetFile != null ? await GetCurrencyFromCacheFile(targetFile, cancellationToken) : null;
        }

        //поиск кэш-файла по дате
        private async Task<Currency[]> GetCachedCurrenciesOnDate(DateOnly date, CancellationToken cancellationToken)
        {
            var pathFile = AppSettings.PathFile;
            var search = ".json";
            string cacheFile;
            var file = Directory.EnumerateFiles(pathFile, "*.json");
            foreach (var item in file)
            {
                cacheFile = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
                var cacheFileToDate = DateTime.ParseExact(cacheFile, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
                if (cacheFileToDate.ToShortDateString() == date.ToShortDateString())
                {
                    return await GetCurrencyFromCacheFile(cacheFile, cancellationToken);
                }
            }

            return null;
        }

        public async Task<CacheTaskDTO> AddNewBaseCurrencyToCacheTaskAsync(string newBaseCurrency, CancellationToken cancellationToken)
        {
            var entity = new CacheTask
            {
                Id = Guid.NewGuid(),
                CacheTackStatus = CacheTackStatus.Created,
                NewBaseCurrency = newBaseCurrency,
                Created = DateTime.UtcNow
            };

            await _internalApiContext.CacheTasks.AddAsync(entity, cancellationToken);
            await _internalApiContext.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<CacheTaskDTO>(entity);

            return dto;
        }

        public async Task<CacheTaskDTO> GetTaskFromCacheTaskAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _internalApiContext.CacheTasks.Where(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<CacheTaskDTO>(entity);
        }

        public async Task ChangeStatusTaskToCacheTaskAsync(CacheTaskDTO cacheTaskDTO, CancellationToken cancellationToken)
        {
            var entity = await _internalApiContext.CacheTasks.Where(e => e.Id == cacheTaskDTO.Id).FirstOrDefaultAsync(cancellationToken);
            entity.CacheTackStatus = (CacheTackStatus)cacheTaskDTO.CacheTackStatus;
            _internalApiContext.CacheTasks.Update(entity);
            await _internalApiContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveNewCacheCurrenciesAsync(List<CurrenciesDTO> currencies, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<List<Currencies>>(currencies);
            await _internalApiContext.CurrenciesList.AddRangeAsync(entity, cancellationToken);
            await _internalApiContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<CacheTaskDTO>> GetUnfinishedTasksAsync(CancellationToken cancellationToken)
        {
            var tasks = await _internalApiContext.CacheTasks
                .Where(c => c.CacheTackStatus == CacheTackStatus.Created || c.CacheTackStatus == CacheTackStatus.InProcessing)
                .Select(e => _mapper.Map<CacheTaskDTO>(e))
                .ToListAsync(cancellationToken);

            return tasks;
        }
    }
}
