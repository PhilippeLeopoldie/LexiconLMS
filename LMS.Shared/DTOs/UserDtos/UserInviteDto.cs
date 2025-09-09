using LMS.Shared.Enums;

namespace LMS.Shared.DTOs.UserDtos;
public record UserInviteDto(string Email, string FirstName, string LastName, UserRole Role, int? CourseId = null);
