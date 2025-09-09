using LMS.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.UserDtos;

public record UserUpdateDto([Required] string UserName,
                            [EmailAddress] string? Email,
                            string? FirstName,
                            string? LastName,
                            string? PhoneNumber,
                            UserRole? Role,
                            int? CourseId);


