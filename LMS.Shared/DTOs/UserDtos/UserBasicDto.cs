namespace LMS.Shared.DTOs.UserDtos;

public record UserBasicDto
{
    public string Id { get; set; } 
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
