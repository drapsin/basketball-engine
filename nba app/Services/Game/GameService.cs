using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Dtos.Game;
using nba_mvc.Repositories.Game;
using nba_mvc.Repositories.Player;
using nba_mvc.Repositories.Referee;

namespace nba_mvc.Services.Game
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IRefereeRepository _refereeRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;

        public GameService(
            IGameRepository gameRepository,
            IRefereeRepository refereeRepository,
            IPlayerRepository playerRepository,
            IMapper mapper)
        {
            _gameRepository = gameRepository;
            _refereeRepository = refereeRepository;
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        public async Task<GameDto?> GetByIdAsync(Guid id)
        {
            var game = await _gameRepository.GetByIdAsync(id);
            return game is null ? null : _mapper.Map<GameDto>(game);
        }

        public async Task<GameDetailDto?> GetDetailByIdAsync(Guid id)
        {
            var game = await _gameRepository.GetByIdWithDetailsAsync(id);
            return game is null ? null : _mapper.Map<GameDetailDto>(game);
        }

        public async Task<IEnumerable<GameDto>> GetAllAsync()
        {
            var games = await _gameRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<GameDto>>(games);
        }

        public async Task<IEnumerable<GameDto>> GetByTeamIdAsync(Guid teamId)
        {
            var games = await _gameRepository.GetByTeamIdAsync(teamId);
            return _mapper.Map<IEnumerable<GameDto>>(games);
        }

        public async Task<GameDetailDto?> CreateAsync(GameCreateDto dto)
        {
            if (dto.RefereeIds.Count != 3)
                throw new InvalidOperationException("A game must have exactly 3 referees.");

            var game = _mapper.Map<Models.Game>(dto);

            var referees = await _refereeRepository.GetByIdsAsync(dto.RefereeIds);
            var players = await _playerRepository.GetByIdsAsync(dto.PlayerIds);

            game.Referees = referees;
            game.Players = players;

            await _gameRepository.AddAsync(game);
            await _gameRepository.SaveChangesAsync();

            var created = await _gameRepository.GetByIdWithDetailsAsync(game.Id);
            return _mapper.Map<GameDetailDto>(created);
        }

        public async Task<bool> UpdateAsync(Guid id, GameUpdateDto dto)
        {
            if (dto.RefereeIds.Count != 3)
                throw new InvalidOperationException("A game must have exactly 3 referees.");

            var game = await _gameRepository.GetByIdWithDetailsAsync(id);
            if (game is null) return false;

            _mapper.Map(dto, game);

            var referees = await _refereeRepository.GetByIdsAsync(dto.RefereeIds);
            var players = await _playerRepository.GetByIdsAsync(dto.PlayerIds);

            game.Referees = referees;
            game.Players = players;

            try
            {
                _gameRepository.Update(game);
                return await _gameRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var game = await _gameRepository.GetByIdAsync(id);
            if (game is null) return false;

            _gameRepository.Delete(game);
            return await _gameRepository.SaveChangesAsync();
        }
    }
}