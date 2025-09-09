using LMS.Shared.Enums;

namespace LMS.Shared.DTOs.UserDtos;

public record UserBasicDto(string Id,
                          string UserName,
                          string Email,
                          string? FirstName,
                          string? LastName,
                          string? PhoneNumber,
                          UserRole Role = UserRole.Student,
                          int? CourseId = null,
                          bool HasPassword = false
                         );
