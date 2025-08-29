using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Enums;
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

    public async Task<(IEnumerable<CourseDto>, MetaData)> GetAllCoursesAsync(
        UserRole? includeUsers = null,
        bool includeModules = false,
        bool includeActivities = false,
        RequestParams requestParams = null!,
        bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));

        PagedList<Course> pagedList = await unitOfWork.CourseRepository.GetAllCoursesAsync(
            includeUsers,
            includeModules,
            includeActivities,
            requestParams,
            trackChanges
            );
        var courses = mapper.Map<IEnumerable<CourseDto>>(pagedList.Items);

        return (courses, pagedList.MetaData);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(
        int courseId,
        UserRole? includeUsers = null,
        bool includeModules = false,
        bool includeActivities = false,
        RequestParams requestParams = null!,
        bool trackChanges = false)
    {
        var query = unitOfWork.CourseRepository
            .FindByCondition(c => c.Id == courseId);

        // include Users
        if (includeUsers == UserRole.Student)
            query = query.Include(course => course.Students);

        if (includeUsers == UserRole.Teacher)
            query = query.Include(course => course.Teachers);

        if (includeUsers == UserRole.All)
        {
            query = query.Include(course => course.Students);
            query = query.Include(course => course.Teachers);
        }
        
        // include Modules and Activities
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

    public async Task AddStudentToCourseAsync(string userId, int courseId, bool trackChanges = true)
    {
        (ApplicationUser user, Course course) = await UserAndCourseValidation(userId, courseId, trackChanges, UserRole.Student);

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains(UserRole.Student.ToString()))
            throw new UserIsNotStudentException(userId);

        if (course.Students.Any(student => student.Id == userId))
            throw new DuplicateStudentInCourseException(userId, courseId);

        course.Students.Add(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task AddTeacherToCourseAsync(string userId, int courseId, bool trackChanges = true)
    {
        (ApplicationUser user, Course course) = await UserAndCourseValidation(userId, courseId, trackChanges, UserRole.Teacher);

        var roles = await userManager.GetRolesAsync(user); 
        if(!roles.Contains(UserRole.Teacher.ToString()))
            throw new UserIsNotTeacherException(userId);
        
        if (course.Teachers.Any(teacher => teacher.Id == userId))
            throw new DuplicateTeacherInCourseException(userId, courseId);

        course.Teachers.Add(user);
        await unitOfWork.CompleteAsync();
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

    private async Task<(ApplicationUser user, Course course)> UserAndCourseValidation(
        string userId,
        int courseId,
        bool trackChanges,
        UserRole userRole = 0
        )
    {
        var user = await userManager.FindByIdAsync(userId)
                    ?? throw new UserNotFoundException(userId);

        var course = await unitOfWork.CourseRepository.GetCourseByIdByUserRoleAsync(courseId, userRole, trackChanges)
            ?? throw new CourseNotFoundException(courseId);

        return (user, course);
    }
}
