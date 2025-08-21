namespace LMS.Shared.DTOs.ActivityDtos;

public record ActivityTypeDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}