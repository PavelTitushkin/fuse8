using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IRepositories;
using InternalApi.Contracts;
using InternalApi.Exceptions;
using InternalApi.Models.ModelResponse;
using InternalApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    /// <summary>
    /// Сервис для работы с currencyApi
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly ICurrencyRepository _currencyRepository;
        public AppSettings AppSettings { get; }

        public CurrencyRateService(IOptions<AppSettings> options, ICurrencyRepository currencyRepository)
        {
            AppSettings = options.Value;
            _currencyRepository = currencyRepository;
        }

        public async Task<Currency> GetCurrencyAsync(CancellationToken cancellationToken)
        {
            await IsHaveException(cancellationToken);
            var dataApiContent = await _currencyRepository.GetCurrencyRateAsync(cancellationToken);
            var data = dataApiContent.Data[AppSettings.Default];

            return new Currency
            {
                Code = data.Code,
                Value = Math.Round(data.Value, AppSettings.Round)
            };
        }

        public async Task<Currency> GetCurrencyAsync(string currencyCode, CancellationToken cancellationToken)
        {
            await IsHaveException(cancellationToken);

            var dataApiContent = await _currencyRepository.GetCurrencyRateAsync(currencyCode, cancellationToken);
            var data = dataApiContent.Data[currencyCode];

            return new Currency()
            {
                Code = data.Code,
                Value = Math.Round(data.Value, AppSettings.Round),
            };
        }

        public async Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date, CancellationToken cancellationToken)
        {
            await IsHaveException(cancellationToken);

            var dataApiContent = await _currencyRepository.GetCurrencyOnDateRateAsync(currencyCode, date, cancellationToken);
            var data = dataApiContent.Data[currencyCode];

            return new CurrencyWithDate()
            {
                Date = dataApiContent.Meta.LastUpdatedAt.ToString("yyyy-MM-dd"),
                Code = data.Code,
                Value = Math.Round(data.Value, AppSettings.Round),
            };
        }

        public async Task<CurrencySettings> GetCurrencySettingsAsync(CancellationToken cancellationToken)
        {
            var dataApiContent = await _currencyRepository.GetCurrencySettingsAsync(cancellationToken);

            return new CurrencySettings
            {
                DefaultCurrency = AppSettings.Default,
                BaseCurrency = AppSettings.Base,
                RequestLimit = dataApiContent.Quotas.Month.Total,
                RequestCount = dataApiContent.Quotas.Month.Used,
                CurrencyRoundCount = AppSettings.Round
            };
        }

        public async Task<Currency[]> GetAllCurrentCurrenciesAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            await IsHaveException(cancellationToken);

            var currenciesRate = await _currencyRepository.GetCurrenciesRateAsync(baseCurrency, cancellationToken);
            var currencies = new Currency[currenciesRate.Data.Count];
            int index = 0;
            foreach (var item in currenciesRate.Data)
            {
                currencies[index] = new Currency()
                {
                    Code = item.Value.Code,
                    Value = Math.Round(item.Value.Value, AppSettings.Round)
                };
                index++;
            }

            return currencies;
        }

        public async Task<CurrenciesOnDate> GetAllCurrenciesOnDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            await IsHaveException(cancellationToken);

            var currenciesRate = await _currencyRepository.GetCurrenciesOnDateRateAsync(baseCurrency, date, cancellationToken);
            var currencies = new Currency[currenciesRate.Data.Count];
            int index = 0;
            foreach (var item in currenciesRate.Data)
            {
                currencies[index] = new Currency()
                {
                    Code = item.Value.Code,
                    Value = Math.Round(item.Value.Value, AppSettings.Round)
                };
                index++;
            }

            return new CurrenciesOnDate()
            {
                Currencies = currencies,
                LastUpdatedAt = currenciesRate.Meta.LastUpdatedAt
            };
        }

        private async Task<bool> IsCurrencyLimitExceededAsync(CancellationToken cancellationToken)
        {
            var apiSettings = await GetCurrencySettingsAsync(cancellationToken);

            return apiSettings.RequestLimit < apiSettings.RequestCount;
        }

        private async Task IsHaveException(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);
            if (await IsCurrencyLimitExceededAsync(cancellationToken))
                throw new ApiRequestLimitException("Превышено количество запросов.");
        }
    }
}
