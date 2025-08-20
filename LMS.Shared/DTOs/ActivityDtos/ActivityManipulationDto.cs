using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.ActivityDtos;
public record ActivityManipulationDto
{
    [Required(ErrorMessage = "The Name is required")]
    public required string Name { get; init; }
    public string? Description { get; init; }
    [Required(ErrorMessage = "The Start date is required")]
    public required DateTime StartsAt { get; init; }
    [Required(ErrorMessage = "The End date is required")]
    public required DateTime EndsAt { get; init; }
    [Required(ErrorMessage = "The Activity type is required")]
    public required int ActivityTypeId { get; set; }
}
