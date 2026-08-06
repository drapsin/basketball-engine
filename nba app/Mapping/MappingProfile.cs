using AutoMapper;
using nba_mvc.Models;
using nba_mvc.Dtos.Arena;

namespace nba_mvc.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Arena
            CreateMap<Arena, ArenaDto>();
            CreateMap<ArenaCreateDto, Arena>();
            CreateMap<ArenaUpdateDto, Arena>();
        }
    }
}