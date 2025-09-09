using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.DashboardDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System.Security.Claims;

namespace LMS.Services;
public class DashboardService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : ServiceBase, IDashboardService
{
    public async Task<StudentDashboardStatsDto> GetStudentStatsAsync(ClaimsPrincipal user)
    {
        var loggedInUser = await userManager.GetUserAsync(user);

        if (loggedInUser == null)
        {
            return new StudentDashboardStatsDto();
        }
        var activityTypeId = await unitOfWork.ActivityTypeRepository.GetAssignmentTypeIdAsync()
            ?? throw new NotFoundException("Kan inte hitta aktivitets typ");
        int modulesCount = await unitOfWork.ModuleRepository.GetModulesCountAsync(loggedInUser.CourseId);
        var assignmentsCount = await unitOfWork.ActivityRepository.GetAssignmentCountAsync(activityTypeId, loggedInUser.CourseId);
        var upcomingActivitiesCount = await unitOfWork.ActivityRepository.FindByCondition(a => a.StartsAt > DateTime.Now && a.Module.CourseId == loggedInUser.CourseId).CountAsync();
        var passedModulesCount = await unitOfWork.ModuleRepository.GetPassedModulesCountAsync(loggedInUser.CourseId);

        return new StudentDashboardStatsDto
        {
            ModulesCount = modulesCount.ToString(),
            PassedModulesCount = passedModulesCount.ToString(),
            AssignmentsCount = assignmentsCount.ToString(),
            UpcomingActivitiesCount = upcomingActivitiesCount.ToString(),
        };
    }

    public async Task<TeacherDashboardStatsDto> GetTeacherStatsAsync(ClaimsPrincipal user)
    {
        var loggedInUser = await userManager.GetUserAsync(user);

        if (loggedInUser == null)
        {
            return new TeacherDashboardStatsDto();
        }
        var activityTypeId = await unitOfWork.ActivityTypeRepository.GetAssignmentTypeIdAsync()
            ?? throw new NotFoundException("Kan inte hitta aktivitets typ");
        var coursesCount = await unitOfWork.CourseRepository.GetActiveCoursesCountAsync();
        var studentsCount = await unitOfWork.CourseRepository.GetActiveStudentsCountAsync();
        var assignmentsCount = await unitOfWork.ActivityRepository.GetAssignmentCountAsync(activityTypeId, null);
        var upcomingActivitiesCount = await unitOfWork.ActivityRepository.FindByCondition(a => a.StartsAt > DateTime.Now).CountAsync();

        return new TeacherDashboardStatsDto
        {
            CoursesCount = coursesCount.ToString(),
            StudentsCount = studentsCount.ToString(),
            AssignmentsCount = assignmentsCount.ToString(),
            UpcomingActivitiesCount = upcomingActivitiesCount.ToString(),
        };
    }
}
