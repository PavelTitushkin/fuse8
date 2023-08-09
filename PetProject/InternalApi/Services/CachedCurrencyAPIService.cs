using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Services
{
    public class CachedCurrencyAPIService : ICachedCurrencyAPI
    {
        private readonly ICachedCurrencyRepository _cachedCurrencyRepository;
        private readonly ICurrencyRateService _currencyRateService;

        public CachedCurrencyAPIService(ICachedCurrencyRepository cachedCurrencyRepository, ICurrencyRateService currencyRateService)
        {
            _cachedCurrencyRepository = cachedCurrencyRepository;
            _currencyRateService = currencyRateService;
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
                var currencies = await _currencyRateService.GetAllCurrentCurrenciesAsync(currencyType.ToString(), cancellationToken);
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
                var currenciesOnDate = await _currencyRateService.GetAllCurrenciesOnDateAsync(currencyType.ToString(), date, cancellationToken);
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
