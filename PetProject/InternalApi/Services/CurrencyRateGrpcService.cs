﻿using AutoMapper;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Abstractions;
using Grpc.Core;
using InternalApi;
using InternalApi.Contracts;
using InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Services
{
    public class CurrencyRateGrpcService : CurrrncyGrpsService.CurrrncyGrpsServiceBase
    {
        private readonly ICachedCurrencyAPI _cachedCurrencyAPI;
        private readonly ICurrencyRateService _currencyRateService;
        private readonly IMapper _mapper;

        public CurrencyRateGrpcService(ICachedCurrencyAPI cachedCurrencyAPI, IMapper mapper, ICurrencyRateService currencyRateService)
        {
            _cachedCurrencyAPI = cachedCurrencyAPI;
            _mapper = mapper;
            _currencyRateService = currencyRateService;
        }

        public override async Task<CurrencyResponse> GetCurrency(CurrencyRequest currency, ServerCallContext context)
        {
            Enum.TryParse(currency.CurrencyCode, out CurrencyType currencyType);
            var currencies = await _cachedCurrencyAPI.GetCurrentCurrencyAsync(currencyType, context.CancellationToken);
            var currencyResponse = _mapper.Map<CurrencyResponse>(currencies);

            return currencyResponse;
        }

        public override async Task<CurrencyResponse> GetCurrencyOnDate(CurrencyOnDateRequest currencyOnDate, ServerCallContext context)
        {
            Enum.TryParse(currencyOnDate.CurrencyCode, out CurrencyType currencyType);
            var date = DateOnly.FromDateTime(currencyOnDate.Date.ToDateTime());
            var currencies = await _cachedCurrencyAPI.GetCurrencyOnDateAsync(currencyType, date, context.CancellationToken);
            var currencyResponse = _mapper.Map<CurrencyResponse>(currencies);

            return currencyResponse;
        }

        public override async Task<ApiSettingsResponse> GetSettingsApi(ApiSettingsRequest settings, ServerCallContext context)
        {
            var apiSettings = await _currencyRateService.GetCurrencySettingsAsync();

            return new ApiSettingsResponse
            {
                BaseCode = apiSettings.BaseCurrency,
                Limit = apiSettings.RequestLimit > apiSettings.RequestCount
            };
        }
    }
}
