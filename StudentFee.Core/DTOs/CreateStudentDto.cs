namespace StudentFee.Core.DTOs;

public class CreateStudentDto
{
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}