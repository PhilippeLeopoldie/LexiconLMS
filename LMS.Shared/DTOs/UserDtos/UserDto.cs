namespace LMS.Shared.DTOs.UserDtos;

public record UserDto
{
    public string Id { get; init; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? FullName { get; set; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int? CourseId { get; init; }
}
