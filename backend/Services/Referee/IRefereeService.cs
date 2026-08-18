using nba_mvc.Dtos.Referee;

namespace nba_mvc.Services.Referee
{
    public interface IRefereeService
    {
        Task<RefereeDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<RefereeDto>> GetAllAsync();
        Task<RefereeDto> CreateAsync(RefereeCreateDto dto);
        Task<bool> UpdateAsync(Guid id, RefereeUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}