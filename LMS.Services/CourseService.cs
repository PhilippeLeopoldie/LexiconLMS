using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class CourseService(IUnitOfWork unitOfWork, IMapper mapper) : ICourseService
{
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync() =>
        await unitOfWork.CourseRepository
            .FindAll()
            .ProjectTo<CourseDto>(mapper.ConfigurationProvider)
            .ToListAsync();

    public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
    {
        var result = await unitOfWork.CourseRepository
            .FindByCondition(c => c.Id == courseId)
            .FirstOrDefaultAsync();
        if (result == null)
            return null;
        return mapper.Map<CourseDto>(result);
    }

    public async Task<CourseDto> GetCourseForUserAsync(string userId)
    {
        //var result = await unitOfWork.CourseRepository
        //    .FindByCondition(c => c.Users.Any(u => u.Id == userId))
        //    .FirstOrDefaultAsync();
        // Implementation to create a new course
        throw new NotImplementedException();
    }
}
