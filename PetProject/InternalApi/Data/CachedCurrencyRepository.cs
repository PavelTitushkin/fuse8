using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace InternalApi.Data
{
    public class CachedCurrencyRepository : ICachedCurrencyRepository
    {
        public AppSettings AppSettings { get; }

        public CachedCurrencyRepository(IOptions<AppSettings> options)
        {
            AppSettings = options.Value;
        }

        //Запись Currency[] в кэш-файл
        public async Task WriteCurrenciesToCacheFileAsync(Currency[] currencies, CancellationToken cancellationToken)
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
        public async Task WriteCurrenciesOnDateToCacheFileAsync(Currency[] currencies, DateOnly date, CancellationToken cancellationToken)
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

        //Получени Currency из кэш-файла
        public async Task<Currency[]> GetCurrencyFromCacheFile(string cacheFile, CancellationToken cancellationToken)
        {
            var pathFile = AppSettings.PathFile;
            var filePathJson = Path.Combine(pathFile, $"{cacheFile}.json");
            var currensiesText = await File.ReadAllTextAsync(filePathJson, cancellationToken);
            var currensies = JsonSerializer.Deserialize<Currency[]>(currensiesText);

            return currensies;
        }

        //Поиск кэш-файла
        public string FindCacheFile()
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

            return cacheDictionary.OrderBy(x => x.Value).FirstOrDefault(x => x.Value < cacheLifetime).Key;
        }

        //поиск кэш-файла по дате
        public string FindCacheFileOnDate(DateOnly date)
        {
            var pathFile = AppSettings.PathFile;
            var search = ".json";
            string cacheFile = null;
            var file = Directory.EnumerateFiles(pathFile, "*.json");
            foreach (var item in file)
            {
                cacheFile = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
                var cacheFileToDate = DateTime.ParseExact(cacheFile, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
                if (cacheFileToDate.ToShortDateString() == date.ToShortDateString())
                {
                    return cacheFile;
                }
            }

            return cacheFile;
        }
        #region
        //private readonly ICurrencyRepository _currencyRepository;
        //public AppSettings AppSettings { get; }

        //public CachedCurrencyRepository(IOptions<AppSettings> options, ICurrencyRepository currencyRepository)
        //{
        //    AppSettings = options.Value;
        //    _currencyRepository = currencyRepository;
        //}

        //public async Task<CurrencyRateResponse> GetCurrencyOnDateAsync(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        //{
        //    if (cancellationToken.IsCancellationRequested)
        //    {
        //        throw new OperationCanceledException(cancellationToken);
        //    }

        //    var pathFile = AppSettings.PathFile;
        //    var cacheFile = FindCacheFileOnDate(pathFile, date);

        //    return cacheFile != null ? await GetCurrencyFromCacheFile(currencyType, pathFile, cacheFile, cancellationToken) :
        //        await WriteCurrenciesInCacheFileAndGetCurrencyFromApiAsync(currencyType, cancellationToken);
        //}

        //public async Task<CurrencyRateResponse> GetCurrentCurrencyAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        //{
        //    if (cancellationToken.IsCancellationRequested)
        //        throw new OperationCanceledException(cancellationToken);

        //    var pathFile = AppSettings.PathFile;
        //    var cacheFile = FindCacheFile(pathFile);
        //    if (cacheFile != null)
        //        return await GetCurrencyFromCacheFile(currencyType, pathFile, cacheFile, cancellationToken);
        //    else
        //        return await WriteCurrenciesInCacheFileAndGetCurrencyFromApiAsync(currencyType, cancellationToken);
        //}

        ////Запись Currency[] в кэш-файл
        //private async Task<CurrencyRateResponse> WriteCurrenciesInCacheFileAndGetCurrencyFromApiAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        //{
        //    var baseValue = AppSettings.Base;

        //    var currencyResponse = await _currencyRepository.GetCurrenciesRateAsync(baseValue, cancellationToken);

        //    //var currencies = await _currencyRepository.GetAllCurrentCurrenciesAsync(baseValue, cancellationToken);
        //    var path = Path.Combine(AppSettings.PathFile, DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm") + ".json");
        //    await using FileStream createStream = File.Create(path);
        //    await JsonSerializer.SerializeAsync(createStream, currencyResponse);
        //    await createStream.DisposeAsync();

        //    //

        //    return currencyResponse;
        //}

        ////получени Currency из кэш-файла
        //private async Task<CurrencyRateResponse> GetCurrencyFromCacheFile(CurrencyType currencyType, string pathFile, string cacheFile, CancellationToken cancellationToken)
        //{
        //    var filePathJson = Path.Combine(pathFile, $"{cacheFile}.json");
        //    var currensiesText = await File.ReadAllTextAsync(filePathJson, cancellationToken);
        //    var currensies = JsonSerializer.Deserialize<CurrencyRateResponse>(currensiesText);
        //    //var currency = currensies.FirstOrDefault(x => x.Code == currencyType.ToString().ToUpper());

        //    //var dto = new CurrencyDTO()
        //    //{
        //    //    CurrencyType = currencyType,
        //    //    Value = currency.Value
        //    //};
        //    return currensies;
        //}

        ////поиск кэш-файла
        //private string FindCacheFile(string pathFile)
        //{
        //    var dateNow = DateTime.UtcNow;
        //    var search = ".json";
        //    var cacheLifetime = AppSettings.CacheLifetime;
        //    var file = Directory.EnumerateFiles(pathFile, "*.json");
        //    var cacheDictionary = new Dictionary<string, double>();
        //    foreach (var item in file)
        //    {
        //        var fileName = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
        //        var fileNameToDate = DateTime.ParseExact(fileName, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
        //        cacheDictionary[fileName] = (dateNow - fileNameToDate).TotalHours;
        //    }

        //    return cacheDictionary.OrderBy(x => x.Value).FirstOrDefault(x => x.Value < cacheLifetime).Key;
        //}

        ////поиск кэш-файла по дате
        //private string FindCacheFileOnDate(string pathFile, DateOnly date)
        //{
        //    var search = ".json";
        //    string fileName = null;
        //    var file = Directory.EnumerateFiles(pathFile, "*.json");
        //    foreach (var item in file)
        //    {
        //        fileName = item.Substring(pathFile.Length + 1, item.Length - pathFile.Length - 1 - search.Length);
        //        var fileNameToDate = DateTime.ParseExact(fileName, "yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture);
        //        if (fileNameToDate.ToShortDateString() == date.ToShortDateString())
        //            return fileName;
        //    }

        //    return fileName;
        //}
        #endregion
    }
}
