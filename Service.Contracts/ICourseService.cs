using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Enums;

namespace Service.Contracts;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto>, MetaData metaData)> GetAllCoursesAsync(
        UserRole? includeUsers = null,
        bool includeModules = false,
        bool includeActivities = false,
        RequestParams requestParams = null!,
        bool trackChanges = false);

    Task<CourseDto?> GetCourseByIdAsync(
        int courseId,
        UserRole? includeUsers = null,
        bool includeModules = false,
        bool includeActivities = false,
        RequestParams requestParams = null!,
        bool trackChanges = false);

    Task<(CourseDto?, MetaData)> GetCourseForUserAsync(string userId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false);
    Task<(CourseDto courseDto, int createdCourseId)> CreateCourseAsync(CourseForModificationDto courseDto);
    Task AddStudentToCourseAsync(string userId, int courseId, bool trackChanges = true);
    Task AddTeacherToCourseAsync(string userId, int courseId, bool trackChanges = true);
    Task UpdateCourseAsync(int courseId, CourseForModificationDto courseDto);
    Task DeleteCourseAsync(int courseId);
}