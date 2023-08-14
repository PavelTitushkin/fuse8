using InternalApi.Contracts;
using InternalApi.Exceptions;
using InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    public class CachedCurrencyAPI : ICachedCurrencyAPI
    {
        private readonly ICachedCurrencyRepository _cachedCurrencyRepository;
        public AppSettings AppSettings { get; }

        public CachedCurrencyAPI(ICachedCurrencyRepository cachedCurrencyRepository, IOptions<AppSettings> options)
        {
            _cachedCurrencyRepository = cachedCurrencyRepository;
            AppSettings = options.Value;
        }

        public async Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var currencies = await _cachedCurrencyRepository.GetCurrentCurrenciesAsync(cancellationToken);
            var targetCurrency = currencies.FirstOrDefault(x => x.Code == currencyType.ToString().ToUpper());

            if (targetCurrency == null)
            {
                throw new CurrencyNotFoundException("Валюта не найдена.");
            }

            return new CurrencyDTO
            {
                CurrencyType = currencyType,
                Value = targetCurrency.Value
            };
        }

        public async Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            var currencies = await _cachedCurrencyRepository.GetCurrenciesOnDateAsync(date, cancellationToken);
            var targetCurrency = currencies.FirstOrDefault(x => x.Code == currencyType.ToString().ToUpper());
            if (targetCurrency == null)
            {
                throw new CurrencyNotFoundException("Валюта не найдена.");
            }

            return new CurrencyDTO
            {
                CurrencyType = currencyType,
                Value = targetCurrency.Value
            };
        }
    }
}
