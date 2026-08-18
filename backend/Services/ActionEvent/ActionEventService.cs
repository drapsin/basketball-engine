using AutoMapper;
using nba_mvc.Dtos.ActionEvent;
using nba_mvc.Repositories.ActionEvent;

namespace nba_mvc.Services.ActionEvent
{
    public class ActionEventService : IActionEventService
    {
        private readonly IActionEventRepository _repository;
        private readonly IMapper _mapper;

        public ActionEventService(IActionEventRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActionEventDto>> GetByGameIdAsync(Guid gameId)
        {
            var events = await _repository.GetByGameIdAsync(gameId);
            return _mapper.Map<IEnumerable<ActionEventDto>>(events);
        }

        public async Task<ActionEventDto> CreateAsync(ActionEventCreateDto dto)
        {
            var actionEvent = _mapper.Map<Models.ActionEvent>(dto);
            await _repository.AddAsync(actionEvent);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(actionEvent.Id);
            return _mapper.Map<ActionEventDto>(created);
        }
    }
}