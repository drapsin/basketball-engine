using nba_mvc.Dtos.ActionEvent;

namespace nba_mvc.Services.ActionEvent
{
    public interface IActionEventService
    {
        Task<IEnumerable<ActionEventDto>> GetByGameIdAsync(Guid gameId);
        Task<ActionEventDto> CreateAsync(ActionEventCreateDto dto);
    }
}