using AutoMapper;
using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Services;

public class CourseService(IMapper mapper) : ICourseService
{
    public Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        // Implementation to get all courses
        throw new NotImplementedException();
    }

    public Task<CourseDto> GetCourseByIdAsync(int courseId)
    {
        // Implementation to get a course by ID
        throw new NotImplementedException();
    }

    public Task<CourseDto> GetCourseForUserAsync(string userId)
    {
        // Implementation to create a new course
        throw new NotImplementedException();
    }
}
