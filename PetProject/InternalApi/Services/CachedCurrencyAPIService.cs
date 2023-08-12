using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelsConfig;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Services
{
    public class CachedCurrencyAPIService : ICachedCurrencyAPI
    {
        private readonly ICachedCurrencyRepository _cachedCurrencyRepository;
        private readonly ICurrencyRateService _currencyRateService;
        public AppSettings AppSettings { get; }

        public CachedCurrencyAPIService(ICachedCurrencyRepository cachedCurrencyRepository, ICurrencyRateService currencyRateService, IOptions<AppSettings> options)
        {
            _cachedCurrencyRepository = cachedCurrencyRepository;
            _currencyRateService = currencyRateService;
            AppSettings = options.Value;
        }

        public async Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cacheFile = _cachedCurrencyRepository.FindCacheFile();
            if (cacheFile == null)
            {
                var currency = AppSettings.Base;
                var currencies = await _currencyRateService.GetAllCurrentCurrenciesAsync(currency, cancellationToken);
                await _cachedCurrencyRepository.WriteCurrenciesToCacheFileAsync(currencies, cancellationToken);

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
            else
            {
                return await GetCurrencyDTO(currencyType, cacheFile, cancellationToken);
            }
        }

        public async Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var cacheFile = _cachedCurrencyRepository.FindCacheFileOnDate(date);
            if (cacheFile == null)
            {
                var currency = AppSettings.Base;
                var currenciesOnDate = await _currencyRateService.GetAllCurrenciesOnDateAsync(currency, date, cancellationToken);
                var currencies = currenciesOnDate.Currencies;
                await _cachedCurrencyRepository.WriteCurrenciesOnDateToCacheFileAsync(currencies, date, cancellationToken);

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
            else
            {
                return await GetCurrencyDTO(currencyType, cacheFile, cancellationToken);
            }
        }

        private async Task<CurrencyDTO> GetCurrencyDTO(CurrencyType currencyType, string cacheFile, CancellationToken cancellationToken)
        {
            var currencies = await _cachedCurrencyRepository.GetCurrencyFromCacheFile(cacheFile, cancellationToken);
            var currency = currencies.FirstOrDefault(x => x.Code == currencyType.ToString().ToUpper());
            var currencyDto = new CurrencyDTO()
            {
                CurrencyType = currencyType,
                Value = currency.Value
            };

            return currencyDto;
        }
    }
}
