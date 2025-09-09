using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.AuthDtos;
public record UserRegistrationDto
{
    [Required]
    public string Password { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string UserName { get; init; } = string.Empty;

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }

}