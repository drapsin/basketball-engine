using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;
using nba_mvc.Dtos.Player;
using nba_mvc.Repositories.Player;

namespace nba_mvc.Services.Player
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public PlayerService(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PlayerDto?> GetByIdAsync(Guid id)
        {
            var player = await _repository.GetByIdAsync(id);
            return player is null ? null : _mapper.Map<PlayerDto>(player);
        }

        public async Task<IEnumerable<PlayerDto>> GetAllAsync()
        {
            var players = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PlayerDto>>(players);
        }

        public async Task<IEnumerable<PlayerDto>> GetByTeamIdAsync(Guid teamId)
        {
            var players = await _repository.GetByTeamIdAsync(teamId);
            return _mapper.Map<IEnumerable<PlayerDto>>(players);
        }

        public async Task<PlayerDto> CreateAsync(PlayerCreateDto dto)
        {
            var player = _mapper.Map<Models.Player>(dto);
            await _repository.AddAsync(player);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(player.Id);
            return _mapper.Map<PlayerDto>(created);
        }

        public async Task<bool> UpdateAsync(Guid id, PlayerUpdateDto dto)
        {
            var player = await _repository.GetByIdAsync(id);
            if (player is null) return false;

            _mapper.Map(dto, player);

            try
            {
                _repository.Update(player);
                return await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var player = await _repository.GetByIdAsync(id);
            if (player is null) return false;

            _repository.Delete(player);
            return await _repository.SaveChangesAsync();
        }
    }
}