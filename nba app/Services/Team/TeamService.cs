using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Dtos.Team;
using nba_mvc.Models;
using nba_mvc.Repositories.Team;

namespace nba_mvc.Services.Team
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repository;
        private readonly IMapper _mapper;

        public TeamService(ITeamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TeamDto?> GetByIdAsync(Guid id)
        {
            var team = await _repository.GetByIdAsync(id);
            return team is null ? null : _mapper.Map<TeamDto>(team);
        }

        public async Task<TeamDetailDto?> GetDetailByIdAsync(Guid id)
        {
            var team = await _repository.GetByIdWithPlayersAsync(id);
            return team is null ? null : _mapper.Map<TeamDetailDto>(team);
        }

        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            var teams = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<IEnumerable<TeamDto>> GetByConferenceAsync(Conference conference)
        {
            var teams = await _repository.GetByConferenceAsync(conference);
            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<TeamDto> CreateAsync(TeamCreateDto dto)
        {
            var team = _mapper.Map<Models.Team>(dto);
            await _repository.AddAsync(team);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(team.Id);
            return _mapper.Map<TeamDto>(created);
        }

        public async Task<bool> UpdateAsync(Guid id, TeamUpdateDto dto)
        {
            var team = await _repository.GetByIdAsync(id);
            if (team is null) return false;

            _mapper.Map(dto, team);

            try
            {
                _repository.Update(team);
                return await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var team = await _repository.GetByIdAsync(id);
            if (team is null) return false;

            _repository.Delete(team);
            return await _repository.SaveChangesAsync();
        }
    }
}