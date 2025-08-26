using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.CourseDtos;

public record CourseForModificationDto
{
    [Required(ErrorMessage = "The Name is required")]
    public required string Name { get; init; }
    public required string Description { get; init; }
    [Required(ErrorMessage = "The Start date is required")]
    public required DateTime Starts { get; init; }
    public required DateTime Ends { get; init; }
}
