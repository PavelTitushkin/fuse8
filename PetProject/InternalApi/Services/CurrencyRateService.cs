using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelsConfig;
using InternalApi.Contracts;
using InternalApi.Models.ModelResponse;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
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

        public async Task<Currency> GetCurrencyAsync()
        {
            if (await IsCurrencyLimitExceededAsync())
                throw new ApiRequestLimitException("Превышено количество запросов.");

            var round = AppSettings.Round;
            var defaultCurrencyCode = AppSettings.Default;
            var dataApiContent = await _currencyRepository.GetCurrencyRateAsync();
            var currency = new Currency();
            var data = dataApiContent.Data[defaultCurrencyCode];
            currency.Code = data.Code;
            currency.Value = Math.Round(data.Value, round);

            return currency;
        }

        public async Task<Currency> GetCurrencyAsync(string currencyCode)
        {
            if (await IsCurrencyLimitExceededAsync())
                throw new ApiRequestLimitException("Превышено количество запросов.");
            else
            {
                var dataApiContent = await _currencyRepository.GetCurrencyRateAsync(currencyCode);
                var currency = new Currency();
                var round = AppSettings.Round;
                var data = dataApiContent.Data[currencyCode];
                currency.Code = data.Code;
                currency.Value = Math.Round(data.Value, round);

                return currency;
            }
        }

        public async Task<CurrencyWithDate> GetCurrencyAsync(string currencyCode, DateTime date)
        {
            if (await IsCurrencyLimitExceededAsync())
                throw new ApiRequestLimitException("Превышено количество запросов.");

            var dataApiContent = await _currencyRepository.GetCurrencyOnDateRateAsync(currencyCode, date);
            var currencyWithDate = new CurrencyWithDate();
            var data = dataApiContent.Data[currencyCode];
            var round = AppSettings.Round;
            currencyWithDate.Date = dataApiContent.Meta.LastUpdatedAt.ToString("yyyy-MM-dd");
            currencyWithDate.Code = data.Code;
            currencyWithDate.Value = Math.Round(data.Value, round);

            return currencyWithDate;
        }

        public async Task<CurrencySettings> GetCurrencySettingsAsync()
        {
            var dataApiContent = await _currencyRepository.GetCurrencySettingsAsync();

            var currencySettings = new CurrencySettings();
            var defaultCurrencyCode = AppSettings.Default;
            var baseCurrencyCode = AppSettings.Base;
            var round = AppSettings.Round;

            currencySettings.DefaultCurrency = defaultCurrencyCode;
            currencySettings.BaseCurrency = baseCurrencyCode;
            currencySettings.RequestLimit = dataApiContent.Quotas.Month.Total;
            currencySettings.RequestCount = dataApiContent.Quotas.Month.Used;
            currencySettings.CurrencyRoundCount = round;

            return currencySettings;
        }

        private async Task<bool> IsCurrencyLimitExceededAsync()
        {
            var apiSettings = await GetCurrencySettingsAsync();

            return apiSettings.RequestLimit < apiSettings.RequestCount;
        }

        public async Task<Currency[]> GetAllCurrentCurrenciesAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);
            if (await IsCurrencyLimitExceededAsync())
                throw new ApiRequestLimitException("Превышено количество запросов.");

            var round = AppSettings.Round;
            var currenciesRate = await _currencyRepository.GetCurrenciesRateAsync(baseCurrency, cancellationToken);
            var currencies = new Currency[currenciesRate.Data.Count];
            int index = 0;
            foreach (var item in currenciesRate.Data)
            {
                currencies[index] = new Currency()
                {
                    Code = item.Value.Code,
                    Value = Math.Round(item.Value.Value, round)
                };
                index++;
            }

            return currencies;
        }

        public async Task<CurrenciesOnDate> GetAllCurrenciesOnDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(cancellationToken);
            if (await IsCurrencyLimitExceededAsync())
                throw new ApiRequestLimitException("Превышено количество запросов.");

            var currenciesRate = await _currencyRepository.GetCurrenciesOnDateRateAsync(baseCurrency, date, cancellationToken);
            var currencies = new Currency[currenciesRate.Data.Count];
            int index = 0;
            var round = AppSettings.Round;
            foreach (var item in currenciesRate.Data)
            {
                currencies[index] = new Currency()
                {
                    Code = item.Value.Code,
                    Value = Math.Round(item.Value.Value, round)
                };
                index++;
            }

            return new CurrenciesOnDate()
            {
                Currencies = currencies,
                LastUpdatedAt = currenciesRate.Meta.LastUpdatedAt
            };
        }
    }
}
