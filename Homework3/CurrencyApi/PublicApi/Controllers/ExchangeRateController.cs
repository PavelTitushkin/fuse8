using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Controllers
{
    /// <summary>
    /// Методы получения курсов валют
    /// </summary>
    [Route("CurrencyRate")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ILimitCheckService _limitCheck;
        private readonly ICurrencyRateService _currencyRateService;
        public ExchangeRateController(ILimitCheckService limitCheck, ICurrencyRateService currencyRateService)
        {
            _limitCheck = limitCheck;
            _currencyRateService = currencyRateService;
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
            if (_limitCheck.CheckLimit())
                throw new ApiRequestLimitException("Превышен лимит запросов.");
            else
                return await _currencyRateService.GetCurrencyAsync();
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
            if (_limitCheck.CheckLimit())
                throw new ApiRequestLimitException("Превышен лимит запросов.");
            else
                return await _currencyRateService.GetCurrencyAsync(currencyCode);
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
            if (_limitCheck.CheckLimit())
                throw new ApiRequestLimitException("Превышен лимит запросов.");
            else
                return await _currencyRateService.GetCurrencyAsync(currencyCode, date);
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
            return await _currencyRateService.GetCurrencySettingsAsync();
        }
    } 
}

