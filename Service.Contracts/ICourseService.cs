using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto>, MetaData metaData)> GetAllCoursesAsync(bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false);
    Task<CourseDto?> GetCourseByIdAsync(int courseId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false);
    Task<(CourseDto?, MetaData)> GetCourseForUserAsync(string userId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false);
    Task<(CourseDto courseDto, int createdCourseId)> CreateCourseAsync(CourseForModificationDto courseDto);
    Task UpdateCourseAsync(int courseId, CourseForModificationDto courseDto);
    Task DeleteCourseAsync(int courseId);
}