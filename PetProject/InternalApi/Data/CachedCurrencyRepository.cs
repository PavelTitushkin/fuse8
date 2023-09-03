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
    /// <summary>
    /// Репозитории для работы с кэшем
    /// </summary>
    public class CachedCurrencyRepository : ICachedCurrencyRepository
    {
        private readonly ICurrencyAPI _currencyAPI;
        private readonly InternalApiContext _internalApiContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Конфигурации приложения
        /// </summary>
        public AppSettings AppSettings { get; }


        /// <summary>
        /// <inheritdoc cref="CachedCurrencyRepository"/>
        /// </summary>
        /// <param name="options">Настройки </param>
        /// <param name="currencyAPI"></param>
        /// <param name="currencyRateContext">Контекст БД</param>
        /// <param name="mapper">Маппер</param>
        public CachedCurrencyRepository(IOptions<AppSettings> options, ICurrencyAPI currencyAPI,
            InternalApiContext currencyRateContext,
            IMapper mapper)
        {
            AppSettings = options.Value;
            _currencyAPI = currencyAPI;
            _internalApiContext = currencyRateContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Получает курсы валют из кэша на диске
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Массив курсов валют</returns>
        /// <exception cref="OperationCanceledException">Исключение отмены операции</exception>
        public async Task<Currency[]> GetCurrentCurrenciesAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cachedCurrencies = await GetCachedCurrencies(cancellationToken);
            if (cachedCurrencies == null)
            {
                var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(AppSettings.Base, cancellationToken);
                await WriteCurrenciesToCacheFileAsync(currencies, cancellationToken);

                return currencies;
            }

            return cachedCurrencies;
        }


        /// <summary>
        /// Получает курсы валют на определюнную дату <paramref name="date"/>
        /// </summary>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Массив курсов валют на <paramref name="date"/></returns>
        /// <exception cref="OperationCanceledException">Исключение отмены операции</exception>
        public async Task<Currency[]> GetCurrenciesOnDateAsync(DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cachedCurrencies = await GetCachedCurrenciesOnDate(date, cancellationToken);
            if (cachedCurrencies == null)
            {
                var currenciesOnDate = await _currencyAPI.GetAllCurrenciesOnDateAsync(AppSettings.Base, date, cancellationToken);
                var currencies = currenciesOnDate.Currencies;
                await WriteCurrenciesOnDateToCacheFileAsync(currencies, date, cancellationToken);

                return currencies;
            }

            return cachedCurrencies;
        }

        /// <summary>
        /// Получает курсы валют из БД
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">Исключение отмены операции</exception>
        /// <exception cref="Exception">Исключение</exception>
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

            var tasks = await GetUnfinishedTasksAsync(cancellationToken);

            if (cachedCurrencies == null && tasks.Count == 0)
            {
                return await GetCasheCurrenciesFromAPIAsync(cancellationToken);
            }

            if (cachedCurrencies == null && tasks.Count > 0)
            {
                await Task.Delay(AppSettings.WaitTimeTaskExecution);
                var lastTask = await GetUnfinishedTasksAsync(cancellationToken);
                if (lastTask.Count > 0)
                {
                    throw new Exception("Ошибка. В очереди есть не законченные задачи.");
                }
            }

            return cachedCurrencies;
        }

        /// <summary>
        /// Получает список валют относительно <paramref name="date"/>
        /// </summary>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют на <paramref name="date"/></returns>
        /// <exception cref="OperationCanceledException">Исключение отмены операции</exception>
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

        /// <summary>
        /// Получает все записи курсов валют
        /// </summary>
        /// <param name="newBaseCurrency">Новая базовая валюта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список всех записей курсов валют</returns>
        /// <exception cref="OperationCanceledException">Исключение отмены операции</exception>
        public async Task<List<CurrenciesDTO>> GetAllCurrenciesFromDbAsync(string newBaseCurrency, CancellationToken cancellationToken)
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
                var currencies = await _currencyAPI.GetAllCurrentCurrenciesAsync(newBaseCurrency, cancellationToken);
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

        /// <summary>
        /// Добавляет задачу для пересчёта кэша в БД относительно <paramref name="newBaseCurrency"/> 
        /// </summary>
        /// <param name="newBaseCurrency">Новая базовая валюта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Объект состояния задачи</returns>
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

        /// <summary>
        /// Получает задачу по уникальному индентификатору <paramref name="id"/>
        /// </summary>
        /// <param name="id">Уникальный индентификатор</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Объект состояния задачи</returns>
        public async Task<CacheTaskDTO> GetTaskFromCacheTaskAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _internalApiContext.CacheTasks.Where(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<CacheTaskDTO>(entity);
        }

        /// <summary>
        /// Изменяет статус(состояние) задачи
        /// </summary>
        /// <param name="cacheTaskDTO">Объект состояния задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Изменение статуса(состояния задачи)</returns>
        public async Task ChangeStatusTaskToCacheTaskAsync(CacheTaskDTO cacheTaskDTO, CancellationToken cancellationToken)
        {
            var entity = await _internalApiContext.CacheTasks.Where(e => e.Id == cacheTaskDTO.Id).FirstOrDefaultAsync(cancellationToken);
            entity.CacheTackStatus = (CacheTackStatus)cacheTaskDTO.CacheTackStatus;
            _internalApiContext.CacheTasks.Update(entity);
            await _internalApiContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Сохраняет пересчитанный кэш в БД
        /// </summary>
        /// <param name="currencies">Список курсов валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сохранение результата пересчёта</returns>
        public async Task SaveNewCacheCurrenciesAsync(List<CurrenciesDTO> currencies, CancellationToken cancellationToken)
        {
            var entities = _mapper.Map<List<Currencies>>(currencies);
            _internalApiContext.CurrenciesList.UpdateRange(entities);
            await _internalApiContext.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// Получает не завершенные задачи
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список не завершенных задач</returns>
        public async Task<List<CacheTaskDTO>> GetUnfinishedTasksAsync(CancellationToken cancellationToken)
        {
            var tasks = await _internalApiContext.CacheTasks
                .Where(c => c.CacheTackStatus == CacheTackStatus.Created || c.CacheTackStatus == CacheTackStatus.InProcessing)
                .Select(e => _mapper.Map<CacheTaskDTO>(e))
                .ToListAsync(cancellationToken);

            return tasks;
        }


        /// <summary>
        /// Полчает курсы валют от внешнего Api
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Курсы валют</returns>
        private async Task<CurrenciesDTO> GetCasheCurrenciesFromAPIAsync(CancellationToken cancellationToken)
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

        /// <summary>
        /// Записывает курсы валют в кэш на диск
        /// </summary>
        /// <param name="currencies">Массив курсов валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Запись кэша на диск</returns>
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

        /// <summary>
        /// Записывает курсы валют на определённую <paramref name="date"/> в кэш на диск
        /// </summary>
        /// <param name="date">Дата, на которую нужно записать курс валют</param>
        /// <param name="currencies">Массив курсов валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Запись кэша на диск на определённую <paramref name="date"/></returns>
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

        /// <summary>
        /// Получает список курсов валют из кэш-файла <paramref name="cacheFile"/>
        /// </summary>
        /// <param name="cacheFile">Кэш-файл</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список курсов валют</returns>
        private async Task<Currency[]> GetCurrencyFromCacheFile(string cacheFile, CancellationToken cancellationToken)
        {
            var pathFile = AppSettings.PathFile;
            var filePathJson = Path.Combine(pathFile, $"{cacheFile}.json");
            var currensiesText = await File.ReadAllTextAsync(filePathJson, cancellationToken);
            var currencies = JsonSerializer.Deserialize<Currency[]>(currensiesText);

            return currencies;
        }

        /// <summary>
        /// Поиск кэш-файла на диске 
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        private async Task<Currency[]> GetCachedCurrencies(CancellationToken cancellationToken)
        {
            var pathFile = AppSettings.PathFile;
            var search = ".json";
            var file = Directory.EnumerateFiles(pathFile, "*.json");
            var cacheDictionary = new Dictionary<string, double>();
            foreach (var item in file)
            {
                var fileName = item.Substring(AppSettings.PathFile.Length + 1, item.Length - AppSettings.PathFile.Length - 1 - search.Length);
                var fileNameToDate = DateTime.ParseExact(fileName, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
                cacheDictionary[fileName] = (DateTime.UtcNow - fileNameToDate).TotalHours;
            }

            var targetFile = cacheDictionary.OrderBy(x => x.Value).FirstOrDefault(x => x.Value < AppSettings.CacheLifetime).Key;

            return targetFile != null ? await GetCurrencyFromCacheFile(targetFile, cancellationToken) : null;
        }

        /// <summary>
        /// Поиск кэш-файла на диске по <paramref name="date"/>
        /// </summary>
        /// <param name="date">Дата, на которую нужно получить курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
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
    }
}
