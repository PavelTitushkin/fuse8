using Fuse8_ByteMinds.SummerSchool.InternalApi.Contracts.IQueues;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelResponse;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    /// <summary>
    /// Методы получения курсов валют
    /// </summary>
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyRateService _currencyRateService;
        private readonly ICachedCurrencyAPI _cachedCurrencyAPI;
        private readonly IBackgroundTaskQueue _taskQueue;

        /// <summary>
        /// Кноструктор
        /// </summary>
        /// <param name="currencyRateService">Сервис для работы с курсами валют</param>
        /// <param name="cachedCurrencyAPI">Сервис для работы данными кеша</param>
        /// <param name="taskQueue">Очередь</param>
        public CurrencyController(ICurrencyRateService currencyRateService, ICachedCurrencyAPI cachedCurrencyAPI, IBackgroundTaskQueue taskQueue)
        {
            _currencyRateService = currencyRateService;
            _cachedCurrencyAPI = cachedCurrencyAPI;
            _taskQueue = taskQueue;
        }

        /// <summary>
        /// Получает курс валюты <paramref name="currencyType"/>
        /// </summary>
        /// <param name="currencyType">Валюта, для которой необходимо получить курс</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Курс валюты <paramref name="currencyType"/></returns>
        [HttpGet]
        [Route("currency/{currencyType}")]
        public async Task<IActionResult> GetCurrency(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrentCurrencyAsync(currencyType, cancellationToken);

            return Ok(apiResponse);
        }

        /// <summary>
        /// Получает курс валюты <paramref name="currencyType"/> из БД
        /// </summary>
        /// <param name="currencyType">Валюта, для которой необходимо получить курс</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Курс валюты <paramref name="currencyType"/> из БД</returns>
        [HttpGet]
        [Route("currencyFromDb/{currencyType}")]
        public async Task<IActionResult> GetCurrencyFromDb(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrentCurrencyFromDbAsync(currencyType, cancellationToken);

            return Ok(apiResponse);
        }

        /// <summary>
        /// Получает курс валюты <paramref name="currencyType"/> на <paramref name="date"/>
        /// </summary>
        /// <param name="currencyType">Код валюты относительно которого будет выведен курс</param>
        /// <param name="date">Дата, относительно которой выводится курс</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Курс валюты <paramref name="currencyType"/> на <paramref name="date"/></returns>
        [HttpGet]
        [Route("currency/{currencyType}/{date}")]
        public async Task<IActionResult> GetCurrencyOnDate(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrencyOnDateAsync(currencyType, date, cancellationToken);

            return Ok(apiResponse);
        }

        /// <summary>
        /// Получает курс валюты <paramref name="currencyType"/> на <paramref name="date"/> из БД
        /// </summary>
        /// <param name="currencyType">Код валюты относительно которого будет выведен курс</param>
        /// <param name="date">Дата, относительно которой выводится курс</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Курс валюты <paramref name="currencyType"/> на <paramref name="date"/> из БД</returns>
        [HttpGet]
        [Route("currencyFromDb/{currencyType}/{date}")]
        public async Task<IActionResult> GetCurrencyOnDateFromDb(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            var apiResponse = await _cachedCurrencyAPI.GetCurrencyOnDateFromDbAsync(currencyType, date, cancellationToken);

            return Ok(apiResponse);
        }


        /// <summary>
        /// Получает текущие настройки внешнего Api
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Настройки внешнего Api</returns>
        [HttpGet]
        [Route("settings")]
        public async Task<IActionResult> Settings(CancellationToken cancellationToken)
        {
            var apiResponse = await _currencyRateService.GetCurrencySettingsAsync(cancellationToken);
            var apiSettings = new InternalApiSettings
            {
                Code = apiResponse.BaseCurrency,
                Limit = apiResponse.RequestLimit > apiResponse.RequestCount
            };

            return Ok(apiSettings);
        }

        /// <summary>
        /// Пересчитывает кэш курсов валбт относительно <paramref name="newBaseCurrency"/> новой базовой валюты
        /// </summary>
        /// <param name="newBaseCurrency">Новая базовая валюта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Пересчет кэша курсов валют</returns>
        [HttpGet]
        [Route("recalculateCurrencyCacheToNewBaseCurrency/{newBaseCurrency}")]
        public async Task<IActionResult> RecalculateCurrencyCacheToNewBaseCurrency(string newBaseCurrency, CancellationToken cancellationToken)
        {
            var taskDto = await _cachedCurrencyAPI.AddNewBaseCurrencyToCacheTaskAsync(newBaseCurrency, cancellationToken);
            await _taskQueue.QueueAsync(new WorkItem(taskDto.Id, taskDto.CacheTackStatus));
            
            return Accepted(taskDto.Id);
        }
    }
}

