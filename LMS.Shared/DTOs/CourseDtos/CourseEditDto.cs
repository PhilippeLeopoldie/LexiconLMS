namespace LMS.Shared.DTOs.CourseDtos;

public record CourseEditDto : CourseForModificationDto
{
    public int Id { get; init; }
}
