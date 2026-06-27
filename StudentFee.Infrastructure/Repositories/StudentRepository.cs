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
            FROM students
            ORDER BY createdat DESC
            """;

        return await connection.QueryAsync<Student>(sql);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM students
            WHERE id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Student>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(Student student)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO students
            (
                id,
                name,
                email,
                phone,
                createdat
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
            UPDATE students
            SET
                name = @Name,
                email = @Email,
                phone = @Phone
            WHERE id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, student);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM students
            WHERE id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, new { Id = id });

        return affectedRows > 0;
    }

    public async Task<StudentWithFeesDto?> GetStudentWithFeesAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT 
            s.id, s.name, s.email, s.phone,
            f.id, f.amount, f.paymentdate, f.remarks
        FROM students s
        LEFT JOIN fees f ON s.id = f.studentid
        WHERE s.id = @Id
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
            splitOn: "id"
        );

        return result.FirstOrDefault();
    }

    public async Task<PagedResult<Student>> GetPagedAsync(int page, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT COUNT(*) FROM students;

        SELECT *
        FROM students
        ORDER BY createdat DESC
        LIMIT @Take OFFSET @Skip;
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
        SELECT * FROM students
        WHERE name ILIKE '%' || @Keyword || '%'
        OR email ILIKE '%' || @Keyword || '%'
        """;

        return await connection.QueryAsync<Student>(sql, new { Keyword = keyword });
    }
}