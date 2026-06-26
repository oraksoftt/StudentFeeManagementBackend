using Dapper;
using StudentFee.Core.DTOs;
using StudentFee.Core.Entities;
using StudentFee.Core.Interfaces;
using StudentFee.Infrastructure.Data;

namespace StudentFee.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public StudentRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Students
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryAsync<Student>(sql);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Students
            WHERE Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Student>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(Student student)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Students
            (
                Id,
                Name,
                Email,
                Phone,
                CreatedAt
            )
            VALUES
            (
                @Id,
                @Name,
                @Email,
                @Phone,
                @CreatedAt
            )
            """;

        await connection.ExecuteAsync(sql, student);

        return student.Id;
    }

    public async Task<bool> UpdateAsync(Student student)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE Students
            SET
                Name = @Name,
                Email = @Email,
                Phone = @Phone
            WHERE Id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, student);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM Students
            WHERE Id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, new { Id = id });

        return affectedRows > 0;
    }
    public async Task<StudentWithFeesDto?> GetStudentWithFeesAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = """
        SELECT 
            s.Id, s.Name, s.Email, s.Phone,
            f.Id, f.Amount, f.PaymentDate, f.Remarks
        FROM Students s
        LEFT JOIN Fees f ON s.Id = f.StudentId
        WHERE s.Id = @Id
    """;

        var studentDict = new Dictionary<Guid, StudentWithFeesDto>();

        var result = await connection.QueryAsync<StudentWithFeesDto, FeeDto, StudentWithFeesDto>(
            sql,
            (student, fee) =>
            {
                if (!studentDict.TryGetValue(student.Id, out var current))
                {
                    current = student;
                    current.Fees = new List<FeeDto>();
                    studentDict.Add(current.Id, current);
                }

                if (fee != null && fee.Id != Guid.Empty)
                    current.Fees.Add(fee);

                return current;
            },
            new { Id = id },
            splitOn: "Id"
        );

        return result.FirstOrDefault();

    }
    public async Task<PagedResult<Student>> GetPagedAsync(int page, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = """
        SELECT COUNT(*) FROM Students;

        SELECT * FROM Students
        ORDER BY CreatedAt DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
    """;

        using var multi = await connection.QueryMultipleAsync(sql, new
        {
            Skip = (page - 1) * pageSize,
            Take = pageSize
        });

        var total = await multi.ReadFirstAsync<int>();
        var items = await multi.ReadAsync<Student>();

        return new PagedResult<Student>
        {
            TotalCount = total,
            Items = items
        };

    }
    public async Task<IEnumerable<Student>> SearchAsync(string keyword)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT * FROM Students
        WHERE Name LIKE '%' + @Keyword + '%'
        OR Email LIKE '%' + @Keyword + '%'
    """;

        return await connection.QueryAsync<Student>(sql, new { Keyword = keyword });
    }
}