using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services;

public class CourseService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : ServiceBase, ICourseService
{
    public async Task<(CourseDto courseDto, int createdCourseId)> CreateCourseAsync(CourseForModificationDto courseDto)
    {
        EnsureNotNull(courseDto, "Course data is null.");
        ValidateDateRange(courseDto.Starts, courseDto.Ends);

        var course = mapper.Map<Course>(courseDto);
        unitOfWork.CourseRepository.Create(course);

        if (!ValidateEntity(course, out var errors))
            throw new BadRequestException($"Invalid course data: {errors}");

        try
        {
            unitOfWork.CourseRepository.Create(course);
            await unitOfWork.CompleteAsync();

            return (mapper.Map<CourseDto>(course), course.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while creating the course.", ex);
        }
    }

    public async Task<(IEnumerable<CourseDto>, MetaData)> GetAllCoursesAsync(bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));

        PagedList<Course> pagedList = await unitOfWork.CourseRepository.GetAllCoursesAsync(includeModules, includeActivities, requestParams, trackChanges);
        var courses = mapper.Map<IEnumerable<CourseDto>>(pagedList.Items);

        return (courses, pagedList.MetaData);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int courseId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false)
    {
        var query = unitOfWork.CourseRepository
            .FindByCondition(c => c.Id == courseId);

        if (includeModules)
        {
            query = query.Include(c => c.Modules);
            if (includeActivities)
                query = query.Include(c => c.Modules).ThenInclude(m => m.Activities);
        }
        if (trackChanges)
            query = query.AsTracking();

        var result = await query
            .FirstOrDefaultAsync();

        if (result == null)
            return null;
        return mapper.Map<CourseDto>(result);
    }

    public async Task<(CourseDto?, MetaData)> GetCourseForUserAsync(string userId, bool includeModules = false, bool includeActivities = false, RequestParams requestParams = null!, bool trackChanges = false)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ArgumentException("User not found", nameof(userId));

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
    public async Task UpdateCourseAsync(int courseId, CourseForModificationDto courseDto)
    {
        EnsureNotNull(courseDto, "Course data is null.");
        ValidateDateRange(courseDto.Starts, courseDto.Ends);

        var course = unitOfWork.CourseRepository.FindByCondition(c => c.Id == courseId, true).FirstOrDefault()
            ?? throw new NotFoundException($"Course with ID {courseId} not found.");

        mapper.Map(courseDto, course);
        if (!ValidateEntity(course, out var errors))
            throw new BadRequestException($"Invalid course data: {errors}");

        try
        {
            unitOfWork.CourseRepository.Update(course);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while updating the course.", ex);
        }
    }

    public async Task DeleteCourseAsync(int courseId)
    {
        var course = await unitOfWork.CourseRepository.FindByCondition(c => c.Id == courseId).FirstOrDefaultAsync()
            ?? throw new NotFoundException($"Course with ID {courseId} not found.");

        try
        {
            unitOfWork.CourseRepository.Delete(course);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while deleting the course.", ex);
        }
    }
}
