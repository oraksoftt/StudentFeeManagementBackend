namespace StudentFee.Core.DTOs;

public class StudentWithFeesDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public List<FeeDto> Fees { get; set; } = new();
}

public class FeeDto
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Remarks { get; set; }
}