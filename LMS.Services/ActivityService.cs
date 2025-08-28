using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services;
public class ActivityService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase, IActivityService
{
    public async Task<(IEnumerable<ActivityDto> activities, MetaData metaData)> GetAllAsync(int moduleId, RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        await EnsureModuleExists(moduleId);

        PagedList<Activity> pagedList = await unitOfWork.ActivityRepository.GetModuleActivitiesAsync(moduleId, requestParams, trackChanges);
        var activities = mapper.Map<IEnumerable<ActivityDto>>(pagedList.Items);

        return (activities, pagedList.MetaData);
    }


    public async Task<ActivityDto> GetByIdAsync(int moduleId, int id, bool trackChanges = false)
    {
        await EnsureModuleExists(moduleId);
        var activity = await unitOfWork.ActivityRepository.GetActivityByIdAsync(activity => activity.Id == id && activity.ModuleId == moduleId, trackChanges);
        return activity == null
            ? throw new NotFoundException($"Activity with id '{id}' not found in module '{moduleId}'.")
            : mapper.Map<ActivityDto>(activity);
    }

    public async Task<(ActivityDto activityDto, int createdActivityId)> CreateAsync(int moduleId, ActivityCreateDto activityCreateDto)
    {
        await EnsureModuleExists(moduleId);
        EnsureNotNull(activityCreateDto, "Activity data is null.");
        ValidateDateRange(activityCreateDto.StartsAt, activityCreateDto.EndsAt);
        var activityType = await unitOfWork.ActivityTypeRepository.GetByIdAsync(activityCreateDto.ActivityTypeId)
            ?? throw new NotFoundException($"Activity Type with ID {activityCreateDto.ActivityTypeId} not found.");

        var module = await unitOfWork.ModuleRepository.GetModuleByConditionAsync(module => module.Id == moduleId, false, false)
            ?? throw new NotFoundException($"Module with id '{moduleId}' not found.");

        EnsureActivityWithinModule(activityCreateDto.StartsAt, activityCreateDto.EndsAt, module);

        if (await unitOfWork.ActivityRepository.AnyOverlappingAsync(moduleId, activityCreateDto.StartsAt, activityCreateDto.EndsAt))
            throw new ActivityOverlapException($"{activityCreateDto.StartsAt:yyyy-MM-dd HH:mm} - {activityCreateDto.EndsAt:yyyy-MM-dd HH:mm}");

        var activity = mapper.Map<Activity>(activityCreateDto);
        activity.ModuleId = moduleId;
        if (!ValidateEntity(activity, out var errors))
            throw new BadRequestException($"Invalid activity data: {errors}");

        try
        {
            unitOfWork.ActivityRepository.Create(activity);
            await unitOfWork.CompleteAsync();

            return (mapper.Map<ActivityDto>(activity), activity.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while creating the activity.", ex);
        }
    }

    public async Task UpdateAsync(int moduleId, int id, ActivityEditDto activityEditDto)
    {
        await EnsureModuleExists(moduleId);
        EnsureNotNull(activityEditDto, "Activity data is null.");
        ValidateDateRange(activityEditDto.StartsAt, activityEditDto.EndsAt);
        var activityType = await unitOfWork.ActivityTypeRepository.GetByIdAsync(activityEditDto.ActivityTypeId)
            ?? throw new NotFoundException($"Activity Type with ID {activityEditDto.ActivityTypeId} not found.");

        var activity = await unitOfWork.ActivityRepository.GetActivityByIdAsync(activity => activity.Id == id && activity.ModuleId == moduleId, true);

        var module = await unitOfWork.ModuleRepository.GetModuleByConditionAsync(module => module.Id == moduleId, false, false)
                      ?? throw new NotFoundException($"Module with id '{moduleId}' not found.");
        EnsureActivityWithinModule(activityEditDto.StartsAt, activityEditDto.EndsAt, module!);

        if (await unitOfWork.ActivityRepository.AnyOverlappingAsync(moduleId, activityEditDto.StartsAt, activityEditDto.EndsAt, id))
            throw new ActivityOverlapException($"{activityEditDto.StartsAt:yyyy-MM-dd HH:mm} - {activityEditDto.EndsAt:yyyy-MM-dd HH:mm}");

        mapper.Map(activityEditDto, activity);
        if (!ValidateEntity(activity, out var errors))
            throw new BadRequestException($"Invalid activity data: {errors}");

        try
        {
            unitOfWork.ActivityRepository.Update(activity!);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while updating the activity.", ex);
        }
    }

    public async Task DeleteAsync(int moduleId, int id)
    {
        await EnsureModuleExists(moduleId);
        var activity = await unitOfWork.ActivityRepository.GetActivityByIdAsync(activity => activity.Id == id && activity.ModuleId == moduleId)
            ?? throw new NotFoundException($"Activity with id '{id}' not found in module '{moduleId}'.");

        try
        {
            unitOfWork.ActivityRepository.Delete(activity);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while checking activity lock status.", ex);
        }
    }

    public async Task<IEnumerable<AssignmentDto>> GetStudentAssignmentsAsync(string studentUserId)
    {
        var assignmentTypeId = await unitOfWork.ActivityTypeRepository.GetAssignmentTypeIdAsync();
        if (assignmentTypeId == null)
            return [];

        var course = await unitOfWork.CourseRepository.GetByStudentIdAsync(studentUserId);
        if (course == null)
            return [];

        var assignments = await unitOfWork.ActivityRepository.GetByCourseIdAndTypeIdAsync(course.Id, assignmentTypeId.Value);

        var assignmentDtos = new List<AssignmentDto>();
        foreach (var assignment in assignments)
        {
            var submittedDocument = await unitOfWork.DocumentRepository.GetDocumentForActivityAndUserAsync(assignment.Id, studentUserId);

            assignmentDtos.Add(new AssignmentDto
            {
                Id = assignment.Id,
                Name = assignment.Name,
                EndDate = assignment.EndsAt,
                ModuleName = assignment.Module.Name,
                CourseName = course.Name,
                IsSubmitted = submittedDocument != null,
                IsLate = submittedDocument != null && submittedDocument.UploadedAt > assignment.EndsAt,
                SubmittedDocumentId = submittedDocument?.Id
            });
        }
        return assignmentDtos;
    }

    private async Task EnsureModuleExists(int moduleId)
    {
        if (moduleId == 0 || !await unitOfWork.ModuleRepository.FindByCondition(module => module.Id == moduleId).AnyAsync())
            throw new NotFoundException($"Module with id '{moduleId}' not found.");
    }

    private static void EnsureActivityWithinModule(DateTime startsAt, DateTime endsAt, Module module)
    {
        if (startsAt < module.StartsAt || endsAt > module.EndsAt)
            throw new BadRequestException("Activity must be within module start and end time.");
    }
}
