using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.ActivityDtos;
public record ActivityManipulationDto
{
    [Required(ErrorMessage = "Namn måste anges")]
    public required string Name { get; init; }
    public string? Description { get; init; }
    [Required(ErrorMessage = "Startdatum måste anges")]
    public required DateTime StartsAt { get; init; }
    [Required(ErrorMessage = "Slutdatum måste anges")]
    public required DateTime EndsAt { get; init; }
    [Required(ErrorMessage = "Aktivitetstyp måste anges")]
    public required int ActivityTypeId { get; init; }
}
