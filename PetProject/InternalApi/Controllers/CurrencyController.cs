using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelResponse;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    /// <summary>
    /// Методы получения курсов валют
    /// </summary>
    //[Route("api")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyRateService _currencyRateService;
        private readonly ICachedCurrencyAPI _cachedCurrencyAPI;
        public CurrencyController(ICurrencyRateService currencyRateService, ICachedCurrencyAPI cachedCurrencyAPI)
        {
            _currencyRateService = currencyRateService;
            _cachedCurrencyAPI = cachedCurrencyAPI;
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
        [Route("currency/{currencyType}")]
        public async Task<IActionResult> GetCurrency(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrentCurrencyAsync(currencyType, cancellationToken);

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("currencyFromDb/{currencyType}")]
        public async Task<IActionResult> GetCurrencyFromDb(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrentCurrencyFromDbAsync(currencyType, cancellationToken);

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
        [Route("currency/{currencyType}/{date}")]
        public async Task<IActionResult> GetCurrencyOnDate(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrencyOnDateAsync(currencyType, date, cancellationToken);

            return Ok(apiResponse);
        }


        ///// <summary>
        ///// Метод возвращает текущие настройки приложения.
        ///// </summary>
        ///// <returns>
        ///// Метод возвращает JSON вида
        ///// {
        /////  "defaultCurrency": "RUB", // текущий курс валют по умолчанию из конфигурации
        /////  "baseCurrency": "USD", // базовая валюта, относительно которой считается курс
        /////  "requestLimit": 300, // общее количество доступных запросов, полученное от внешнего API (quotas->month->total)
        /////  "requestCount": 0, //  количество использованных запросов, полученное от внешнего API (quotas->month->used)
        /////  "currencyRoundCount": 2 // Количество знаков после запятой, до которого следует округлять значение курса валют
        /////}
        ///// </returns>
        [HttpGet]
        [Route("settings")]
        public async Task<IActionResult> Settings()
        {
            var apiResponse = await _currencyRateService.GetCurrencySettingsAsync();
            var apiSettings = new InternalApiSettings
            {
                Code = apiResponse.BaseCurrency,
                Limit = apiResponse.RequestLimit > apiResponse.RequestCount
            };

            return Ok(apiSettings);
        }
    }
}

