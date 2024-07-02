using AutoMapper;
using IdealAPI.Model.DTO;
using PracticeAPI.Model.Domain;
using PracticeAPI.Model.DTO;

namespace PracticeAPI.Mappings
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<RegionDTO, Region>().ReverseMap();
            CreateMap<AddRegionReqDto, Region>().ReverseMap();
            CreateMap<AddWalkReqDto, Walk>().ReverseMap();  
            CreateMap<Walk,WalkDTO>().ReverseMap();
            CreateMap<Difficulty,DifficultyDTO>().ReverseMap();

        }
    }
}
