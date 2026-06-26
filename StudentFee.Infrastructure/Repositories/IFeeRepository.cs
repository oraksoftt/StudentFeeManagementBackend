using StudentFee.Core.Entities;

namespace StudentFee.Core.Interfaces;

public interface IFeeRepository
{
    Task<IEnumerable<FeeList>> GetAllAsync();

    Task<Fee?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Fee fee);

    Task<bool> UpdateAsync(Fee fee);

    Task<bool> DeleteAsync(Guid id);
}