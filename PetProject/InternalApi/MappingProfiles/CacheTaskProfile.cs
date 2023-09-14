using AutoMapper;
using DataInternalApi.Entities;
using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.MappingProfiles
{
    public class CacheTaskProfile : Profile
    {
        public CacheTaskProfile()
        {
            CreateMap<CacheTask, CacheTaskDTO>()
                .ForMember(dto => dto.CacheTackStatus, opt => opt.MapFrom(opt => opt.CacheTackStatus));


            CreateMap<CacheTaskDTO, CacheTask>()
                .ForMember(entity => entity.CacheTackStatus, opt => opt.MapFrom(opt => opt.CacheTackStatus));
        }
    }
}
