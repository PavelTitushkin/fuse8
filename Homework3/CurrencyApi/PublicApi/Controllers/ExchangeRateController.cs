using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Controllers
{
    /// <summary>
    /// Методы получения курсов валют
    /// </summary>
    [Route("CurrencyRate")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ICurrencyRateService _currencyRateService;
        public ExchangeRateController(ICurrencyRateService currencyRateService, AppSettings appSettings)
        {
            _currencyRateService = currencyRateService;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Метод получения курса валюты по умолчанию.
        /// </summary>
        /// <returns>
        /// Метод возвращает JSON вида
        ///        {
        ///  "code": "RUB", // код валюты
        ///  "value": 90.50 // текущий курс относительно доллара
        ///}
        /// </returns>
        [HttpGet]
        [Route("Currency")]
        public async Task<Currency> Currency()
        {
            var apiResponse = await _currencyRateService.GetCurrencyAsync();
            var defaultCurrencyCode = _appSettings.Default;
            var round = _appSettings.Round;

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            var currency = new Currency();
            var data = dataApiContent.Data[defaultCurrencyCode];
            currency.Code = data.Code;
            currency.Value = Math.Round(data.Value, round);

            return currency;
        }

        /// <summary>
        /// Метод получения текущего курс валюты, переданного в качестве параметра
        /// </summary>
        /// <param name="currencyCode">
        /// Код валюты относительно которого будет выведен курс.
        /// </param>
        /// <returns>
        /// Метод возвращает JSON вида
        ///        {
        ///  "code": "RUB", // код валюты
        ///  "value": 90.50 // текущий курс относительно доллара
        ///}
        /// </returns>
        /// <exception cref="ApiRequestLimitException">
        /// Выбрасывается исключение если все доступные запросы исчерпаны.
        /// </exception>
        [HttpGet]
        [Route("Currency/{currencyCode}")]
        public async Task<Currency> Currency(string currencyCode)
        {
            var apiResponse = await _currencyRateService.GetCurrencyAsync(currencyCode);
            var round = _appSettings.Round;

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            var currency = new Currency();
            var data = dataApiContent.Data[currencyCode];
            currency.Code = data.Code;
            currency.Value = Math.Round(data.Value, round);

            return currency;
        }

        /// <summary>
        /// Метод получения курса валюты, переданного в качестве параметра, на определенную дата.
        /// </summary>
        /// <param name="currencyCode">
        /// Код валюты относительно которого будет выведен курс.
        /// </param>
        /// <param name="date">
        /// Курс валюты на определённую дату.
        /// </param>
        /// <returns>       
        /// Метод возвращает JSON вида
        ///{
        ///  "date": "2020-12-25", // дата актуальности курса
        ///  "code": "RUB", // код валюты
        ///  "value": 90.50 // текущий курс относительно доллара
        ///}        
        /// </returns>
        /// <exception cref="ApiRequestLimitException">
        /// Выбрасывается исключение если все доступные запросы исчерпаны.
        /// </exception>
        [HttpGet]
        [Route("Currency/{currencyCode}/{date}")]
        public async Task<CurrencyWithDate> Currency(string currencyCode, DateTime date)
        {
            var apiResponse = await _currencyRateService.GetCurrencyAsync(currencyCode, date);
            var round = _appSettings.Round;

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<CurrencyRateResponse>(apiContent);
            var currencyWithDate = new CurrencyWithDate();
            var data = dataApiContent.Data[currencyCode];
            currencyWithDate.Date = dataApiContent.Meta.Last_updated_at.ToString("yyyy-MM-dd");
            currencyWithDate.Code = data.Code;
            currencyWithDate.Value = Math.Round(data.Value, round);

            return currencyWithDate;
        }

        /// <summary>
        /// Метод возвращает текущие настройки приложения.
        /// </summary>
        /// <returns>
        /// Метод возвращает JSON вида
        /// {
        ///  "defaultCurrency": "RUB", // текущий курс валют по умолчанию из конфигурации
        ///  "baseCurrency": "USD", // базовая валюта, относительно которой считается курс
        ///  "requestLimit": 300, // общее количество доступных запросов, полученное от внешнего API (quotas->month->total)
        ///  "requestCount": 0, //  количество использованных запросов, полученное от внешнего API (quotas->month->used)
        ///  "currencyRoundCount": 2 // Количество знаков после запятой, до которого следует округлять значение курса валют
        ///}
        /// </returns>
        [HttpGet]
        [Route("Settings")]
        public async Task<CurrencySettings> Settings()
        {
            var apiResponse = await _currencyRateService.GetCurrencySettingsAsync();
            var defaultCurrencyCode = _appSettings.Default;
            var baseCurrencyCode = _appSettings.Base;
            var round = _appSettings.Round;

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var dataApiContent = JsonSerializer.Deserialize<SettingsResponse>(apiContent);
            var currencySettings = new CurrencySettings();
            currencySettings.defaultCurrency = defaultCurrencyCode;
            currencySettings.baseCurrency = baseCurrencyCode;
            currencySettings.requestLimit = dataApiContent.Quotas.Month.Total;
            currencySettings.requestCount = dataApiContent.Quotas.Month.Used;
            currencySettings.currencyRoundCount = round;

            return currencySettings;
        }
    }
}

