using LMS.Shared.DTOs.DocumentDtos;

namespace LMS.Shared.DTOs.ActivityDtos;
public record ActivityDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime StartsAt { get; init; }
    public required DateTime EndsAt { get; init; }
    public int ModuleId { get; init; }
    public required ActivityTypeDto ActivityType { get; init; }
    public ICollection<DocumentDto> Documents { get; init; } = [];
}
