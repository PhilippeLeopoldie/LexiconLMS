namespace LMS.Shared.DTOs.ActivityDtos;

public record ActivityTypeDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}