
using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(int courseId);
    Task<CourseDto> GetCourseForUserAsync(string userId);
}