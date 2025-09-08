using Domain.Models.Entities;
using LMS.Shared.DTOs.ActivityDtos;
using System.ComponentModel.DataAnnotations;


namespace LMS.Shared.DTOs.ModuleDtos;

public record ModuleForManipulationDto
{
    [Required(ErrorMessage = "Namn måste anges")]
    public required string Name { get; init; }
    [Required(ErrorMessage = "Beskrivning måste anges")]
    public required string Description { get; init; }
    [Required(ErrorMessage = "Startdatum måste anges")]
    public required DateTime StartsAt { get; init; }
    [Required(ErrorMessage = "Slutdatum måste anges")]
    public required DateTime EndsAt { get; init; }
}
