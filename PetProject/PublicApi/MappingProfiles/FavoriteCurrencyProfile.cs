using AutoMapper;
using DataPublicApi.Entities;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.MappingProfiles
{
    public class FavoriteCurrencyProfile : Profile
    {
        public FavoriteCurrencyProfile()
        {
            CreateMap<FavoriteCurrency, FavoriteCurrencyDTO>();

            CreateMap<FavoriteCurrencyDTO, FavoriteCurrency>();
        }
    }
}
