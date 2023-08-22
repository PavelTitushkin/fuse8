using API_DataBase;
using API_DataBase.Entities;
using AutoMapper;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;
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
        private readonly CurrencyRateContext _currencyRateContext;
        private readonly IMapper _mapper;
        public AppSettings AppSettings { get; }


        public CachedCurrencyRepository(IOptions<AppSettings> options, ICurrencyAPI currencyAPI,
            CurrencyRateContext currencyRateContext,
            IMapper mapper)
        {
            AppSettings = options.Value;
            _currencyAPI = currencyAPI;
            _currencyRateContext = currencyRateContext;
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
            var cachedCurrencies = await _currencyRateContext.CurrenciesList
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

                await _currencyRateContext.CurrenciesList.AddAsync(currenciesToDb, cancellationToken);
                await _currencyRateContext.SaveChangesAsync(cancellationToken);

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
            var cachedCurrencies = await _currencyRateContext.CurrenciesList
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

                await _currencyRateContext.CurrenciesList.AddAsync(currenciesToDb, cancellationToken);
                await _currencyRateContext.SaveChangesAsync(cancellationToken);

                return currenciesDto;
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

    }
}
