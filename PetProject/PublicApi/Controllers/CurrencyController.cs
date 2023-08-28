using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Exceptions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Services;
using Grpc.Core;
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
        [Route("ChangeDefaultCurrency/{defaultCurrency}")]
        public async Task<IActionResult> ChangeDefaultCurrency(string defaultCurrency, CancellationToken cancellationToken)
        {
            await _currencyRateService.ChangeDefaultCurrencyAsync(defaultCurrency, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("ChangeCurrencyRound/{round}")]
        public async Task<IActionResult> ChangeCurrencyRound(int round, CancellationToken cancellationToken)
        {
            await _currencyRateService.ChangeCurrencyRoundAsync(round, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("GetFavoriteCurrency/{currencyName}")]
        public async Task<IActionResult> GetFavoriteCurrency(string currencyName, CancellationToken cancellationToken)
        {
            var apiResponse = await _currencyRateService.GetFavoriteCurrencyAsync(currencyName, cancellationToken);

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("GetAllFavoritesCurrencies")]
        public async Task<IActionResult> GetAllFavoritesCurrencies(CancellationToken cancellationToken)
        {
            var apiResponse = await _currencyRateService.GetAllFavoritesCurrenciesAsync(cancellationToken);

            return Ok(apiResponse);
        }

        [HttpPost]
        [Route("AddNewFavoriteCurrency/{currencyName}/{currency}/{currencyBase}")]
        public async Task<IActionResult> AddNewFavoriteCurrency(string currencyName, string currency, string currencyBase, CancellationToken cancellationToken)
        {
            await _currencyRateService.AddNewFavoriteCurrencyAsync(currencyName, currency, currencyBase, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Route("ChangeFavoriteCurrencyByName/{currencyName}/{changedCurrencyName}/{changedCurrency}/{changedCurrencyBase}")]
        public async Task<IActionResult> ChangeFavoriteCurrencyByName(string currencyName, string changedCurrencyName, string changedCurrency, string changedCurrencyBase, CancellationToken cancellationToken)
        {
            await _currencyRateService.ChangeFavoriteCurrencyByNameAsync(currencyName, changedCurrencyName, changedCurrency, changedCurrencyBase, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteFavoriteCurrencyByName/{currencyName}")]
        public async Task<IActionResult> DeleteFavoriteCurrencyByName(string currencyName, CancellationToken cancellationToken)
        {
            await _currencyRateService.DeleteFavoriteCurrencyByNameAsync(currencyName, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("GetCurrencyRateFavoriteByName/{currencyName}")]
        public async Task<IActionResult> GetCurrencyRateFavoriteByName(string currencyName, CancellationToken cancellationToken) 
        {
            var apiResponse = await _currencyRateGrpcClientService.GetCurrencyFavoriteByName(currencyName, cancellationToken);

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("GetCurrencyRateFavoriteByNameOnDate/{currencyName}/{date}")]
        public async Task<IActionResult> GetCurrencyRateFavoriteByNameOnDate(string currencyName, DateOnly date, CancellationToken cancellationToken) 
        {
            var apiResponse = await _currencyRateGrpcClientService.GetCurrencyFavoriteByNameOnDate(currencyName, date, cancellationToken);

            return Ok(apiResponse);
        }
    }
}

