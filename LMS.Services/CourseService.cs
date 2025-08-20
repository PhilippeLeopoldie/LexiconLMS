using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class CourseService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : ICourseService
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
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));
        return await unitOfWork.CourseRepository
            .FindByCondition(c => c.Id == user.CourseId)
            .ProjectTo<CourseDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }
}
