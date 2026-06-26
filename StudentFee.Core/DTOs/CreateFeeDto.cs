namespace StudentFee.Core.DTOs;

public class CreateFeeDto
{
    public Guid StudentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Remarks { get; set; }
}