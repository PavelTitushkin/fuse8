using AutoMapper;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;
using DataInternalApi.Entities;

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
