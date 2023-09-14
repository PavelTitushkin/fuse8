using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using PublicClientApi;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    /// <summary>
    /// Сервис для работы gRPC-клиентом
    /// </summary>
    public class CurrencyRateGrpcClientService
    {
        private readonly CurrrncyGrpsService.CurrrncyGrpsServiceClient _grpcServiceClient;
        private readonly ICurrencyRateService _currencyRateService;
        public AppSettings AppSettings { get; }

        /// <summary>
        /// <inheritdoc cref="CurrencyRateGrpcClientService"/>
        /// </summary>
        /// <param name="options">Конфигурации приложения</param>
        /// <param name="grpsServiceClient">gRPC-клиент</param>
        /// <param name="currencyRateService"><inheritdoc cref="CurrencyRateService"/></param>
        public CurrencyRateGrpcClientService(IOptions<AppSettings> options, CurrrncyGrpsService.CurrrncyGrpsServiceClient grpsServiceClient, ICurrencyRateService currencyRateService)
        {
            AppSettings = options.Value;
            _grpcServiceClient = grpsServiceClient;
            _currencyRateService = currencyRateService;
        }

        /// <summary>
        /// Получает курс валюты относительно <paramref name="currencyCode"/>
        /// </summary>
        /// <param name="currencyCode">Код валюты</param>
        /// <returns>Курс валюты относительно <paramref name="currencyCode"/></returns>
        public async Task<CurrencyResponse> GetCurrency(string currencyCode)
        {
            var getCurrencyRequest = new CurrencyRequest { CurrencyCode = currencyCode };

            return await _grpcServiceClient.GetCurrencyAsync(getCurrencyRequest);
        }

        /// <summary>
        /// Получает курс валюты относительно <paramref name="currencyCode"/> на <paramref name="date"/>
        /// </summary>
        /// <param name="currencyCode">Код валюты</param>
        /// <param name="date">Дата, относительно которой выводится курс валют</param>
        /// <returns>Курс валюты относительно <paramref name="currencyCode"/> на <paramref name="date"/></returns>
        public async Task<CurrencyResponse> GetCurrencyOnDate(string currencyCode, DateTime date)
        {
            var getCurrencyOnDateRequest = new CurrencyOnDateRequest
            {
                CurrencyCode = currencyCode,
                Date = Timestamp.FromDateTime(date.ToUniversalTime())
            };

            return await _grpcServiceClient.GetCurrencyOnDateAsync(getCurrencyOnDateRequest);
        }

        /// <summary>
        /// Получает настройки Api
        /// </summary>
        /// <returns><inheritdoc cref="ApiSettings"/></returns>
        public async Task<ApiSettings> GetApiSettings()
        {
            var apiSettingsRequest = new ApiSettingsRequest { };

            var response = await _grpcServiceClient.GetSettingsApiAsync(apiSettingsRequest);
            var apiSettings = new ApiSettings
            {
                DefaultCurrency = AppSettings.Default,
                BaseCurrency = response.BaseCode,
                NewRequestsAvailable = response.Limit,
                CurrencyRoundCount = AppSettings.Round
            };

            return apiSettings;
        }

        /// <summary>
        /// Получает Избранную валюту <paramref name="currencyName"/>
        /// </summary>
        /// <param name="currencyName">Название избранной валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Избранную валюту</returns>
        public async Task<CurrencyFavoriteResponse> GetCurrencyFavoriteByName(string currencyName, CancellationToken cancellationToken)
        {
            var dto = await _currencyRateService.GetFavoriteCurrencyAsync(currencyName, cancellationToken);
            var getCurrencyFavoriteByNameRequest = new CurrencyFavoriteRequest
            {
                Currency = dto.Currency,
                BaseCurrency = dto.BaseCurrency,
            };

            return await _grpcServiceClient.GetCurrencyFavoriteByNameAsync(getCurrencyFavoriteByNameRequest, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Получает Избранную валюту <paramref name="currencyName"/> на <paramref name="date"/>
        /// </summary>
        /// <param name="currencyName">Название избранной валюты</param>
        /// <param name="date">Дата, относительно которой выводится курс валют</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Курс избранной валюты <paramref name="currencyName"/> на <paramref name="date"/></returns>
        public async Task<CurrencyFavoriteOnDateResponse> GetCurrencyFavoriteByNameOnDate(string currencyName, DateOnly date, CancellationToken cancellationToken)
        {
            var dto = await _currencyRateService.GetFavoriteCurrencyAsync(currencyName, cancellationToken);
            var getCurrencyFavoriteByNameOnDate = new CurrencyFavoriteOnDateRequest
            {
                Currency = dto.Currency,
                BaseCurrency = dto.BaseCurrency,
                Date = Timestamp.FromDateTime(date.ToDateTime(TimeOnly.MinValue).ToUniversalTime()),
            };

            return await _grpcServiceClient.GetCurrencyFavoriteByNameOnDateAsync(getCurrencyFavoriteByNameOnDate, cancellationToken: cancellationToken);
        }
    }
}
