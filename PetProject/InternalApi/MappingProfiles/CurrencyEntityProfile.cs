using DataStore.InternalApiDb.Entities;
using AutoMapper;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;

namespace InternalApi.MappingProfiles
{
    public class CurrencyEntityProfile : Profile
    {
        public CurrencyEntityProfile()
        {
            CreateMap<CurrencyEntity, CurrencyEntityDTO>();

            CreateMap<CurrencyEntityDTO, CurrencyEntity>();
        }
    }
}
