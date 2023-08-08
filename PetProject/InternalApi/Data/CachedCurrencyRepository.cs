using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelsConfig;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace InternalApi.Data
{
    public class CachedCurrencyRepository : ICachedCurrencyAPI
    {
        private readonly ICurrencyRateService _currencyRateService;
        public AppSettings AppSettings { get; }

        public CachedCurrencyRepository(IOptions<AppSettings> options, ICurrencyRateService currencyRateService)
        {
            AppSettings = options.Value;
            _currencyRateService = currencyRateService;
        }

        public async Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);

            var pathFile = AppSettings.PathFile;
            var cacheFile = FindCacheFileOnDate(pathFile, date);
            if (cacheFile != null)
                return await GetCurrencyFromCacheFile(currencyType, pathFile, cacheFile, cancellationToken);
            else
                return await WriteCurrenciesInCacheFileAndGetCurrencyFromApiAsync(currencyType, cancellationToken);
        }

        public async Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);

            var pathFile = AppSettings.PathFile;
            var cacheFile = FindCacheFile(pathFile);
            if (cacheFile != null)
                return await GetCurrencyFromCacheFile(currencyType, pathFile, cacheFile, cancellationToken);
            else
                return await WriteCurrenciesInCacheFileAndGetCurrencyFromApiAsync(currencyType, cancellationToken);
        }

        //Запись Currency[] в кэш-файл
        private async Task<CurrencyDTO> WriteCurrenciesInCacheFileAndGetCurrencyFromApiAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            var baseValue = AppSettings.Base;
            var currencies = await _currencyRateService.GetAllCurrentCurrenciesAsync(baseValue, cancellationToken);
            var path = Path.Combine(AppSettings.PathFile, DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm") + ".json");
            await using FileStream createStream = File.Create(path);
            await JsonSerializer.SerializeAsync(createStream, currencies);
            await createStream.DisposeAsync();

            var currencyDto = new CurrencyDTO();
            foreach (var item in currencies)
            {
                if (item.Code == currencyType.ToString().ToUpper())
                {
                    currencyDto.CurrencyType = currencyType;
                    currencyDto.Value = item.Value;
                    break;
                }
            }

            return currencyDto;
        }

        //получени Currency из кэш-файла
        private async Task<CurrencyDTO> GetCurrencyFromCacheFile(CurrencyType currencyType, string pathFile, string cacheFile, CancellationToken cancellationToken)
        {
            var filePathJson = Path.Combine(pathFile, $"{cacheFile}.json");
            var currensiesText = await File.ReadAllTextAsync(filePathJson, cancellationToken);
            var currensies = JsonSerializer.Deserialize<List<Currency>>(currensiesText);
            var currency = currensies.FirstOrDefault(x => x.Code == currencyType.ToString().ToUpper());

            var dto = new CurrencyDTO()
            {
                CurrencyType = currencyType,
                Value = currency.Value
            };
            return dto;
        }

        //поиск кэш-файла
        private string FindCacheFile(string pathFile)
        {
            var dateNow = DateTime.UtcNow;
            var search = ".json";
            var cacheLifetime = AppSettings.CacheLifetime;
            var file = Directory.EnumerateFiles(pathFile, "*.json");
            var cachDictionary = new Dictionary<string, double>();
            foreach (var item in file)
            {
                var fileName = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
                var fileNameToDate = DateTime.ParseExact(fileName, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
                cachDictionary[fileName] = (dateNow - fileNameToDate).TotalHours;
            }

            return cachDictionary.OrderBy(x => x.Value).FirstOrDefault(x => x.Value < cacheLifetime).Key;
        }

        //поиск кэш-файла по дате
        private string FindCacheFileOnDate(string pathFile, DateOnly date)
        {
            var search = ".json";
            string fileName = null;
            var file = Directory.EnumerateFiles(pathFile, "*.json");
            foreach (var item in file)
            {
                fileName = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
                var fileNameToDate = DateTime.ParseExact(fileName, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
                if (fileNameToDate.ToShortDateString() == date.ToShortDateString())
                    return fileName;
            }

            return fileName;
        }
    }
}
