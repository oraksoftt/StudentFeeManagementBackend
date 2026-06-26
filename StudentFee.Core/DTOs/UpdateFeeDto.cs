namespace StudentFee.Core.DTOs;

public class UpdateFeeDto
{
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Remarks { get; set; }
}