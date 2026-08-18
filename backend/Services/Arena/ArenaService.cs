using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Dtos.Arena;
using nba_mvc.Repositories.Arena;

namespace nba_mvc.Services.Arena
{
    public class ArenaService : IArenaService
    {
        private readonly IArenaRepository _repository;
        private readonly IMapper _mapper;

        public ArenaService(IArenaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ArenaDto?> GetByIdAsync(Guid id)
        {
            var arena = await _repository.GetByIdAsync(id);
            return arena is null ? null : _mapper.Map<ArenaDto>(arena);
        }

        public async Task<IEnumerable<ArenaDto>> GetAllAsync()
        {
            var arenas = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ArenaDto>>(arenas);
        }

        public async Task<ArenaDto> CreateAsync(ArenaCreateDto dto)
        {
            var arena = _mapper.Map<Models.Arena>(dto);
            await _repository.AddAsync(arena);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ArenaDto>(arena);
        }

        public async Task<bool> UpdateAsync(Guid id, ArenaUpdateDto dto)
        {
            var arena = await _repository.GetByIdAsync(id);
            if (arena is null) return false;

            _mapper.Map(dto, arena);

            try
            {
                _repository.Update(arena);
                return await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var arena = await _repository.GetByIdAsync(id);
            if (arena is null) return false;

            _repository.Delete(arena);
            return await _repository.SaveChangesAsync();
        }
    }
}