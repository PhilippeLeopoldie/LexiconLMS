namespace LMS.Shared.DTOs.ActivityDtos;
public record AssignmentDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime EndDate { get; init; }
    public string ModuleName { get; init; } = null!;
    public string CourseName { get; init; } = null!;
    public bool IsSubmitted { get; init; }
    public bool IsLate { get; init; }
    public int? SubmittedDocumentId { get; init; }
}
