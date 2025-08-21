using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.DocumentDtos;
public record DocumentManipulationDto
{
    [Required(ErrorMessage = "The Name is required")]
    public required string Name { get; init; }
    public string? Description { get; init; }

    [Required(ErrorMessage = "The Storage Path is required")]
    public required string StoragePath { get; init; }
    public int Size { get; init; }

    public int? CourseId { get; init; }
    public int? ModuleId { get; init; }
    public int? ActivityId { get; init; }
    public string? FileType { get; init; }
}
