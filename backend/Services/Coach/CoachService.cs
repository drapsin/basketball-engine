using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Dtos.Coach;
using nba_mvc.Repositories.Coach;

namespace nba_mvc.Services.Coach
{
    public class CoachService : ICoachService
    {
        private readonly ICoachRepository _repository;
        private readonly IMapper _mapper;

        public CoachService(ICoachRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CoachDto?> GetByIdAsync(Guid id)
        {
            var coach = await _repository.GetByIdAsync(id);
            return coach is null ? null : _mapper.Map<CoachDto>(coach);
        }

        public async Task<IEnumerable<CoachDto>> GetAllAsync()
        {
            var coaches = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CoachDto>>(coaches);
        }

        public async Task<CoachDto?> GetByTeamIdAsync(Guid teamId)
        {
            var coach = await _repository.GetByTeamIdAsync(teamId);
            return coach is null ? null : _mapper.Map<CoachDto>(coach);
        }

        public async Task<CoachDto> CreateAsync(CoachCreateDto dto)
        {
            var coach = _mapper.Map<Models.Coach>(dto);
            await _repository.AddAsync(coach);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(coach.Id);
            return _mapper.Map<CoachDto>(created);
        }

        public async Task<bool> UpdateAsync(Guid id, CoachUpdateDto dto)
        {
            var coach = await _repository.GetByIdAsync(id);
            if (coach is null) return false;

            _mapper.Map(dto, coach);

            try
            {
                _repository.Update(coach);
                return await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coach = await _repository.GetByIdAsync(id);
            if (coach is null) return false;

            _repository.Delete(coach);
            return await _repository.SaveChangesAsync();
        }
    }
}