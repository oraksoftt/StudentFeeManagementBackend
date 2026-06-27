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
                f.id,
                f.studentid,
                s.name AS studentname,
                f.amount,
                f.paymentdate::text,
                f.remarks
            FROM fees f
            INNER JOIN students s ON f.studentid = s.id
            ORDER BY f.paymentdate DESC
            """;

        return await connection.QueryAsync<FeeList>(sql);
    }

    public async Task<Fee?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM fees
            WHERE id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Fee>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(Fee fee)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO fees
            (
                id,
                studentid,
                amount,
                paymentdate,
                remarks
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
            UPDATE fees
            SET
                amount = @Amount,
                paymentdate = @PaymentDate,
                remarks = @Remarks
            WHERE id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, fee);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            DELETE FROM fees
            WHERE id = @Id
            """;

        var affectedRows =
            await connection.ExecuteAsync(sql, new { Id = id });

        return affectedRows > 0;
    }
}