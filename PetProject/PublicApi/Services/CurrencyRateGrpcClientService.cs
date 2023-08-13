using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelResponse;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using PublicClientApi;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    public class CurrencyRateGrpcClientService
    {
        private readonly CurrrncyGrpsService.CurrrncyGrpsServiceClient _grpcServiceClient;
        public AppSettings AppSettings { get; }

        public CurrencyRateGrpcClientService(IOptions<AppSettings> options, CurrrncyGrpsService.CurrrncyGrpsServiceClient grpsServiceClient)
        {
            AppSettings = options.Value;
            _grpcServiceClient = grpsServiceClient;
        }

        public async Task<CurrencyResponse> GetCurrency(string currencyCode)
        {
            var getCurrencyRequest = new CurrencyRequest { CurrencyCode = currencyCode };
            
            return await _grpcServiceClient.GetCurrencyAsync(getCurrencyRequest);
        }

        public async Task<CurrencyResponse> GetCurrencyOnDate(string currencyCode, DateTime date)
        {
            var getCurrencyOnDateRequest = new CurrencyOnDateRequest
            {
                CurrencyCode = currencyCode,
                Date = Timestamp.FromDateTime(date.ToUniversalTime())
            };

            return await _grpcServiceClient.GetCurrencyOnDateAsync(getCurrencyOnDateRequest);
        }

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
    }
}
