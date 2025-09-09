using LMS.Shared.DTOs.DashboardDtos;
using System.Security.Claims;

namespace Service.Contracts;
public interface IDashboardService
{
    Task<StudentDashboardStatsDto> GetStudentStatsAsync(ClaimsPrincipal user);
    Task<TeacherDashboardStatsDto> GetTeacherStatsAsync(ClaimsPrincipal user);
}
