namespace LMS.Shared.DTOs.CourseDtos;

public record CourseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime Starts { get; init; }
}