using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services;

public class CourseService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : ICourseService
{
    public async Task<(IEnumerable<CourseDto>, MetaData)> GetAllCoursesAsync(RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        
        PagedList<Course> pagedList = await unitOfWork.CourseRepository.GetAllCoursesAsync(requestParams, trackChanges);
        var courses = mapper.Map<IEnumerable<CourseDto>>(pagedList.Items);

        return (courses, pagedList.MetaData);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
    {
        var result = await unitOfWork.CourseRepository
            .FindByCondition(c => c.Id == courseId)
            .FirstOrDefaultAsync();
        if (result == null)
            return null;
        return mapper.Map<CourseDto>(result);
    }

    public async Task<(CourseDto?, MetaData)> GetCourseForUserAsync(string userId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        var query = unitOfWork.CourseRepository
            .FindByCondition(c => c.Id == user.CourseId, trackChanges);
        if (includeModules)
        {
            query = query.Include(c => c.Modules);
            if (includeActivities)
                query = query.Include(c => c.Modules).ThenInclude(m => m.Activities);
        }
        MetaData meta = new(1, 1, 1, 1);
        return (await unitOfWork.CourseRepository
        .FindByCondition(c => c.Id == user.CourseId)
        .ProjectTo<CourseDto>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(),
        meta);
    }
}
