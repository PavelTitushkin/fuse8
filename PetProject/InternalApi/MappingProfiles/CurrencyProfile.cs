using AutoMapper;
using InternalApi;
using InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.MappingProfiles
{
    public class CurrencyProfile : Profile
    {
        protected CurrencyProfile()
        {
            CreateMap<CurrencyDTO, CurrencyResponse>()
                .ForMember(opt => opt.Code, dto => dto.MapFrom(dto => dto.CurrencyType.ToString()))
                .ForMember(opt => opt.Value, dto => dto.MapFrom(dto => dto.Value));
        }
    }
}
