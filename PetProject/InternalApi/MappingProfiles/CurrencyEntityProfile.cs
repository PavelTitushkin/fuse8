using DataStore.InternalApiDb.Entities;
using AutoMapper;
using InternalApi.Models.ModelResponse;

namespace InternalApi.MappingProfiles
{
    public class CurrencyEntityProfile : Profile
    {
        public CurrencyEntityProfile()
        {
            CreateMap<CurrencyEntity, Currency>();

            CreateMap<Currency, CurrencyEntity>();
        }
    }
}
