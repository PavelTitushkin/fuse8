using API_DataBase.Entities;
using AutoMapper;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.MappingProfiles
{
    public class CurrenciesProfile : Profile
    {
        public CurrenciesProfile()
        {
            CreateMap<Currencies, CurrenciesDTO>()
                .ForMember(dto => dto.CurrenciesList, opt => opt.MapFrom(opt => opt.CurrenciesList));

            CreateMap<CurrenciesDTO, Currencies>()
                .ForMember(ent => ent.CurrenciesList, dto => dto.MapFrom(opt => opt.CurrenciesList));
        }
    }
}
