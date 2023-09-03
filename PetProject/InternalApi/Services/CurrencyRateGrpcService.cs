using AutoMapper;
using Grpc.Core;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;
using InternalApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    public class CurrencyRateGrpcService : CurrrncyGrpsService.CurrrncyGrpsServiceBase
    {
        private readonly ICachedCurrencyAPI _cachedCurrencyAPI;
        private readonly ICurrencyRateService _currencyRateService;
        private readonly IMapper _mapper;

        public AppSettings AppSettings { get; set; }

        public CurrencyRateGrpcService(IOptions<AppSettings> options, ICachedCurrencyAPI cachedCurrencyAPI, IMapper mapper, ICurrencyRateService currencyRateService)
        {
            _cachedCurrencyAPI = cachedCurrencyAPI;
            _mapper = mapper;
            _currencyRateService = currencyRateService;
            AppSettings = options.Value;
        }

        public override async Task<CurrencyResponse> GetCurrency(CurrencyRequest currency, ServerCallContext context)
        {
            Enum.TryParse(currency.CurrencyCode.ToUpper(), out CurrencyType currencyType);
            var currencies = await _cachedCurrencyAPI.GetCurrentCurrencyAsync(currencyType, context.CancellationToken);

            return _mapper.Map<CurrencyResponse>(currencies);
        }

        public override async Task<CurrencyResponse> GetCurrencyOnDate(CurrencyOnDateRequest currencyOnDate, ServerCallContext context)
        {
            Enum.TryParse(currencyOnDate.CurrencyCode.ToUpper(), out CurrencyType currencyType);
            var date = DateOnly.FromDateTime(currencyOnDate.Date.ToDateTime());
            var currencies = await _cachedCurrencyAPI.GetCurrencyOnDateAsync(currencyType, date, context.CancellationToken);
            var currencyResponse = _mapper.Map<CurrencyResponse>(currencies);

            return currencyResponse;
        }

        public override async Task<ApiSettingsResponse> GetSettingsApi(ApiSettingsRequest settings, ServerCallContext context)
        {
            var apiSettings = await _currencyRateService.GetCurrencySettingsAsync(context.CancellationToken);

            return new ApiSettingsResponse
            {
                BaseCode = apiSettings.BaseCurrency,
                Limit = apiSettings.RequestLimit > apiSettings.RequestCount
            };
        }

        public override async Task<CurrencyFavoriteResponse> GetCurrencyFavoriteByName(CurrencyFavoriteRequest currencyFavorite, ServerCallContext context)
        {
            if(currencyFavorite.BaseCurrency == AppSettings.Base)
            {
                Enum.TryParse(currencyFavorite.Currency.ToUpper(), out CurrencyType currencyType);
                var dto = await _cachedCurrencyAPI.GetCurrentCurrencyFromDbAsync(currencyType, context.CancellationToken);
                
                return new CurrencyFavoriteResponse
                {
                    Currency = dto.CurrencyType.ToString(),
                    BaseCurrency = currencyFavorite.BaseCurrency,
                    Value = (double)dto.Value
                };
            }
            else
            {
                Enum.TryParse(currencyFavorite.Currency.ToUpper(), out CurrencyType currencyType);
                Enum.TryParse(currencyFavorite.BaseCurrency.ToUpper(), out CurrencyType currencyTypeBase);
                var dtoCurrency = await _cachedCurrencyAPI.GetCurrentCurrencyFromDbAsync(currencyType, context.CancellationToken);
                var dtoBaseCurrency = await _cachedCurrencyAPI.GetCurrentCurrencyFromDbAsync(currencyTypeBase, context.CancellationToken);

                return new CurrencyFavoriteResponse
                {
                    Currency = dtoCurrency.CurrencyType.ToString(),
                    BaseCurrency = currencyFavorite.BaseCurrency,
                    Value = Math.Round((double)(dtoCurrency.Value / dtoBaseCurrency.Value), AppSettings.Round)
                };
            }
        }

        public override async Task<CurrencyFavoriteOnDateResponse> GetCurrencyFavoriteByNameOnDate(CurrencyFavoriteOnDateRequest currencyFavorite, ServerCallContext context)
        {
            if (currencyFavorite.BaseCurrency == AppSettings.Base)
            {
                Enum.TryParse(currencyFavorite.Currency.ToUpper(), out CurrencyType currencyType);
                var date = DateOnly.FromDateTime(currencyFavorite.Date.ToDateTime());
                var dto = await _cachedCurrencyAPI.GetCurrencyOnDateFromDbAsync(currencyType, date, context.CancellationToken);

                return new CurrencyFavoriteOnDateResponse
                {
                    Currency = dto.CurrencyType.ToString(),
                    BaseCurrency = currencyFavorite.BaseCurrency,
                    Value = (double)dto.Value,
                    Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(date.ToDateTime(TimeOnly.MinValue).ToUniversalTime()),
                };
            }
            else
            {
                Enum.TryParse(currencyFavorite.Currency.ToUpper(), out CurrencyType currencyType);
                Enum.TryParse(currencyFavorite.BaseCurrency.ToUpper(), out CurrencyType currencyTypeBase);
                var date = DateOnly.FromDateTime(currencyFavorite.Date.ToDateTime());
                var dtoCurrency = await _cachedCurrencyAPI.GetCurrencyOnDateFromDbAsync(currencyType, date, context.CancellationToken);
                var dtoBaseCurrency = await _cachedCurrencyAPI.GetCurrencyOnDateFromDbAsync(currencyTypeBase, date,  context.CancellationToken);

                return new CurrencyFavoriteOnDateResponse
                {
                    Currency = dtoCurrency.CurrencyType.ToString(),
                    BaseCurrency = currencyFavorite.BaseCurrency,
                    Value = Math.Round((double)(dtoCurrency.Value / dtoBaseCurrency.Value), AppSettings.Round),
                    Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(date.ToDateTime(TimeOnly.MinValue).ToUniversalTime()),
                };
            }
        }
    }
}
