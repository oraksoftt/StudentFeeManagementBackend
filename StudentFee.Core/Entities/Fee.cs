namespace StudentFee.Core.Entities;

public class Fee
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Remarks { get; set; }
}
public class FeeList
{
    public Guid Id { get; set; }

    public Guid StudentID { get; set; }
    public string StudentName { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Remarks { get; set; }
}