using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.CourseDtos;

public record CourseForModificationDto
{
    [Required(ErrorMessage = "Namn måste anges")]
    public required string Name { get; init; }
    public required string Description { get; init; }
    [Required(ErrorMessage = "Startdatum måste anges")]
    public required DateTime Starts { get; init; }
    [Required(ErrorMessage = "Slutdatum måste anges")]
    public required DateTime Ends { get; init; }
}
