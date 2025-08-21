using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LMS.Services;

public class CourseService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : ICourseService
{
    public async Task<(CourseDto courseDto, int createdCourseId)> CreateCourseAsync(CourseForCreationDto courseDto)
    {
        EnsureNotNull(courseDto, "Course data is null.");

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

    public async Task<(IEnumerable<CourseDto>, MetaData)> GetAllCoursesAsync(RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        
        PagedList<Course> pagedList = await unitOfWork.CourseRepository.GetAllCoursesAsync(requestParams, trackChanges);
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
        if  (trackChanges)
            query = query.AsTracking();

        var result = await query
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
    private static void EnsureNotNull<T>(T obj, string message)
    {
        if (obj == null)
            throw new BadRequestException(message);
    }
    protected static bool ValidateEntity<T>(T entity, out string? errors)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var validationContext = new ValidationContext(entity);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);
        errors = string.Join("; ", validationResults.Select(x => x.ErrorMessage ?? string.Empty));
        return isValid;
    }
}
