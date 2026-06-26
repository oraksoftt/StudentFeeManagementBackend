using StudentFee.Core.DTOs;
using StudentFee.Core.Entities;

namespace StudentFee.Core.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();

    Task<Student?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Student student);

    Task<bool> UpdateAsync(Student student);

    Task<bool> DeleteAsync(Guid id);
    Task<StudentWithFeesDto?> GetStudentWithFeesAsync(Guid id);
    Task<PagedResult<Student>> GetPagedAsync(int page, int pageSize);
    Task<IEnumerable<Student>> SearchAsync(string keyword);
}