namespace LMS.Shared.DTOs.ActivityDtos;
public record ActivityDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime StartsAt { get; init; }
    public required DateTime EndsAt { get; init; }
    public required ActivityTypeDto Type { get; init; }
}
