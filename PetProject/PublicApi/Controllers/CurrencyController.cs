using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Controllers
{
    /// <summary>
    /// Методы получения курсов валют
    /// </summary>
    //[Route("api")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyRateService _currencyRateService;
        private readonly CurrencyRateGrpcClientService _currencyRateGrpcClientService;
        public CurrencyController(ICurrencyRateService currencyRateService, CurrencyRateGrpcClientService currencyRateGrpsClientService)
        {
            _currencyRateService = currencyRateService;
            _currencyRateGrpcClientService = currencyRateGrpsClientService;
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
        [Route("currency")]
        public async Task<IActionResult> Currency()
        {
            var apiResponse = await _currencyRateService.GetCurrencyAsync();
            
            return Ok(apiResponse);
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
        [Route("currency/{currencyCode}")]
        public async Task<IActionResult> Currency(string currencyCode)
        {
            var apiResponse = await _currencyRateGrpcClientService.GetCurrency(currencyCode);

            return Ok(apiResponse);
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
        [Route("currency/{currencyCode}/{date}")]
        public async Task<IActionResult> Currency(string currencyCode, DateTime date)
        {
            var apiResponse = await _currencyRateGrpcClientService.GetCurrencyOnDate(currencyCode, date);

            return Ok(apiResponse);
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
        [Route("settings")]
        public async Task<IActionResult> Settings()
        {
            var apiResponse = await _currencyRateGrpcClientService.GetApiSettings();

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("ChangeDefaultCurrency")]
        public async Task<IActionResult> ChangeDefaultCurrency(string defaultCurrency, CancellationToken cancellationToken)
        {
            await _currencyRateService.ChangeDefaultCurrencyAsync(defaultCurrency, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("ChangeCurrencyRound")]
        public async Task<IActionResult> ChangeCurrencyRound(int round, CancellationToken cancellationToken)
        {
            await _currencyRateService.ChangeCurrencyRoundAsync(round, cancellationToken);

            return Ok();
        }

    }
}

