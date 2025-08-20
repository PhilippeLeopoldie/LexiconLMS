
using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Services;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto>, MetaData metaData)> GetAllCoursesAsync(RequestParams requestParams, bool trackChanges = false);
    Task<CourseDto?> GetCourseByIdAsync(int courseId);
    Task<CourseDto?> GetCourseForUserAsync(string userId);
}