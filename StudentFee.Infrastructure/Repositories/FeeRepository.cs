using Dapper;
using StudentFee.Core.Entities;
using StudentFee.Core.Interfaces;
using StudentFee.Infrastructure.Data;

namespace StudentFee.Infrastructure.Repositories;

public class FeeRepository : IFeeRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public FeeRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<FeeList>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
                            SELECT 
                                s.id as StudentID,
                                s.Name as StudentName,
                                f.*
                            FROM Fees f
                            INNER JOIN Students s ON f.StudentID = s.id
                            ORDER BY f.PaymentDate DESC
                            """;

        return await connection.QueryAsync<FeeList>(sql);
    }

    public async Task<Fee?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Fees
            WHERE Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Fee>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(Fee fee)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Fees
            (
                Id,
                StudentId,
                Amount,
                PaymentDate,
                Remarks
            )
            VALUES
            (
                @Id,
                @StudentId,
                @Amount,
                @PaymentDate,
                @Remarks
            )
            """;

        await connection.ExecuteAsync(sql, fee);

        return fee.Id;
    }

    public async Task<bool> UpdateAsync(Fee fee)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            UPDATE Fees
            SET
                Amount = @Amount,
                PaymentDate = @PaymentDate,
                Remarks = @Remarks
            WHERE Id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, fee);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM Fees
            WHERE Id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, new { Id = id });

        return affectedRows > 0;
    }
}