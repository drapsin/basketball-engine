using AutoMapper;
using nba_mvc.Dtos.Arena;
using nba_mvc.Dtos.Coach;
using nba_mvc.Dtos.Game;
using nba_mvc.Dtos.Player;
using nba_mvc.Dtos.Referee;
using nba_mvc.Dtos.Team;
using nba_mvc.Models;

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

            // Coach
            CreateMap<Coach, CoachDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : string.Empty));

            CreateMap<CoachCreateDto, Coach>();
            CreateMap<CoachUpdateDto, Coach>();

            // Referee
            CreateMap<Referee, RefereeDto>();
            CreateMap<RefereeCreateDto, Referee>();
            CreateMap<RefereeUpdateDto, Referee>();

            // Team
            CreateMap<Team, TeamDto>()
                .ForMember(dest => dest.ArenaName, opt => opt.MapFrom(src => src.Arena != null ? src.Arena.ArenaName : string.Empty))
                .ForMember(dest => dest.PlayerCount, opt => opt.MapFrom(src => src.Players != null ? src.Players.Count : 0));

            CreateMap<Team, TeamDetailDto>()
                .IncludeBase<Team, TeamDto>();

            CreateMap<TeamCreateDto, Team>();
            CreateMap<TeamUpdateDto, Team>();

            // Player
            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : string.Empty));

            CreateMap<PlayerCreateDto, Player>();
            CreateMap<PlayerUpdateDto, Player>();

            // Game
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => src.HomeTeam != null ? src.HomeTeam.Name : string.Empty))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => src.AwayTeam != null ? src.AwayTeam.Name : string.Empty))
                .ForMember(dest => dest.ArenaName, opt => opt.MapFrom(src => src.Arena != null ? src.Arena.ArenaName : string.Empty));

            CreateMap<Game, GameDetailDto>()
                .IncludeBase<Game, GameDto>();

            CreateMap<GameCreateDto, Game>()
                .ForMember(dest => dest.Referees, opt => opt.Ignore())
                .ForMember(dest => dest.Players, opt => opt.Ignore());

            CreateMap<GameUpdateDto, Game>()
                .ForMember(dest => dest.Referees, opt => opt.Ignore())
                .ForMember(dest => dest.Players, opt => opt.Ignore());
        }
    }
}