using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    /// <summary>
    /// Сервис для работы с currencyApi
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly IHttpCurrencyRepository _httpCurrencyRepository;
        private readonly ICurrencyRepository _currencyRepository;
        public AppSettings AppSettings { get; }

        public CurrencyRateService(IOptions<AppSettings> options, IHttpCurrencyRepository httpCurrencyRepository, ICurrencyRepository currencyRepository)
        {
            AppSettings = options.Value;
            _httpCurrencyRepository = httpCurrencyRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task<Currency> GetCurrencyAsync()
        {
            if (await IsCurrencyLimitExceededAsync())
                throw new ApiRequestLimitException("Превышено количество запросов.");

            var round = AppSettings.Round;
            var defaultCurrencyCode = AppSettings.Default;
            var dataApiContent = await _httpCurrencyRepository.GetCurrencyRateAsync();
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
                var dataApiContent = await _httpCurrencyRepository.GetCurrencyRateAsync(currencyCode);
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

            var dataApiContent = await _httpCurrencyRepository.GetCurrencyOnDateRateAsync(currencyCode, date);
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
            var dataApiContent = await _httpCurrencyRepository.GetCurrencySettingsAsync();

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

        public async Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken)
        {
            await _currencyRepository.ChangeDefaultCurrencyAsync(defaultCurrency.ToUpper(), cancellationToken);
        }

        public async Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken)
        {
            await _currencyRepository.ChangeCurrencyRoundAsync(round, cancellationToken);
        }


        private async Task<bool> IsCurrencyLimitExceededAsync()
        {
            var apiSettings = await GetCurrencySettingsAsync();

            return apiSettings.RequestLimit < apiSettings.RequestCount;
        }
    }
}
