
using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Services;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto>, MetaData metaData)> GetAllCoursesAsync(RequestParams requestParams, bool trackChanges = false);
    Task<CourseDto?> GetCourseByIdAsync(int courseId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false);
    Task<(CourseDto?, MetaData)> GetCourseForUserAsync(string userId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false);
    Task<(CourseDto courseDto, int createdCourseId)> CreateCourseAsync(CourseForCreationDto courseDto);
}