using AutoMapper;
using InternalApi.Models.ModelDTO;

namespace InternalApi.MappingProfiles
{
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<CurrencyDTO, CurrencyResponse>()
                .ForMember(opt => opt.Code, dto => dto.MapFrom(dto => dto.CurrencyType.ToString()))
                .ForMember(opt => opt.Value, dto => dto.MapFrom(dto => dto.Value));
        }
    }
}
