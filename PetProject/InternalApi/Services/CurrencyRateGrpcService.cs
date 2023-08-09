using AutoMapper;
using Grpc.Core;
using InternalApi;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Services
{
    public class CurrencyRateGrpcService : CurrrncyGrpsService.CurrrncyGrpsServiceBase
    {
        private readonly ICachedCurrencyAPI _cachedCurrencyAPI;
        private readonly IMapper _mapper;

        public CurrencyRateGrpcService(ICachedCurrencyAPI cachedCurrencyAPI, IMapper mapper)
        {
            _cachedCurrencyAPI = cachedCurrencyAPI;
            _mapper = mapper;
        }

        public override Task<CurrencyResponse> GetCurrency(CurrencyRequest currency, ServerCallContext context)
        {
            Enum.TryParse(currency.CurrencyCode, out CurrencyType currencyType);
            var currencies = _cachedCurrencyAPI.GetCurrentCurrencyAsync(currencyType, context.CancellationToken);
            var currencyResponse = _mapper.Map<CurrencyResponse>(currencies);

            return Task.FromResult(currencyResponse);
        }

        public override Task<CurrencyOnDateResponse> GetCurrencyOnDate(CurrencyOnDateRequest currencyOnDate, ServerCallContext context)
        {
            return Task.FromResult(new CurrencyOnDateResponse());
        }

        public override Task<ApiSettingsResponse> GetSettingsApi(ApiSettingsRequest settings, ServerCallContext context)
        {
            return Task.FromResult(new ApiSettingsResponse());
        }
    }
}
