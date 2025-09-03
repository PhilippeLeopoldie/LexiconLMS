using Domain.Models.Entities;
using LMS.Shared.DTOs.ActivityDtos;
using System.ComponentModel.DataAnnotations;


namespace LMS.Shared.DTOs.ModuleDtos;

public record ModuleForManipulationDto
{
    [Required(ErrorMessage = "The Name is required")]
    public required string Name { get; init; }

    public string? Description { get; init; }
    [Required(ErrorMessage = "The Start date is required")]
    public required DateTime StartsAt { get; init; }
    [Required(ErrorMessage = "The End date is required")]
    public required DateTime EndsAt { get; init; }
}
