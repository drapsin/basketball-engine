using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nba_mvc.Data;
using nba_mvc.Dtos.Referee;
using nba_mvc.Repositories.Referee;
using System.Xml.Serialization;

namespace nba_mvc.Services.Referee
{
    public class RefereeService : IRefereeService
    {
        private readonly IRefereeRepository _repository;
        private readonly IMapper _mapper;

        public RefereeService(IRefereeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RefereeDto?> GetByIdAsync(Guid id)
        {
            var referee = await _repository.GetByIdAsync(id);
            return referee is null ? null : _mapper.Map<RefereeDto>(referee);
        }

        public async Task<IEnumerable<RefereeDto>> GetAllAsync()
        {
            var referees = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<RefereeDto>>(referees);
        }

        public async Task<RefereeDto> CreateAsync(RefereeCreateDto dto)
        {
            var referee = _mapper.Map<Models.Referee>(dto);
            await _repository.AddAsync(referee);
            await _repository.SaveChangesAsync();
            return _mapper.Map<RefereeDto>(referee);
        }

        public async Task<bool> UpdateAsync(Guid id, RefereeUpdateDto dto)
        {
            var referee = await _repository.GetByIdAsync(id);
            if (referee is null) return false;

            _mapper.Map(dto, referee);

            try
            {
                _repository.Update(referee);
                return await _repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var referee = await _repository.GetByIdAsync(id);
            if (referee is null) return false;

            _repository.Delete(referee);
            return await _repository.SaveChangesAsync();
        }
    }
}