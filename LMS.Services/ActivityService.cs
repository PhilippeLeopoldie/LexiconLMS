using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using Service.Contracts;
using System.ComponentModel.DataAnnotations;

namespace LMS.Services;
public class ActivityService(IUnitOfWork unitOfWork, IMapper mapper) : IActivityService
{
    public async Task<(IEnumerable<ActivityDto> activities, MetaData metaData)> GetAllAsync(int moduleId, RequestParams requestParams, bool trackChanges = false)
    {
        ArgumentNullException.ThrowIfNull(requestParams, nameof(requestParams));
        EnsureModuleExists(moduleId);

        PagedList<Activity> pagedList = await unitOfWork.ActivityRepository.GetModuleActivitiesAsync(moduleId, requestParams, trackChanges);
        var activities = mapper.Map<IEnumerable<ActivityDto>>(pagedList.Items);

        return (activities, pagedList.MetaData);
    }


    public async Task<ActivityDto> GetByIdAsync(int moduleId, int id, bool trackChanges = false)
    {
        EnsureModuleExists(moduleId);
        var activity = await unitOfWork.ActivityRepository.GetActivityByIdAsync(activity => activity.Id == id && activity.ModuleId == moduleId, trackChanges);
        return activity == null
            ? throw new NotFoundException($"Activity with id '{id}' not found in module '{moduleId}'.")
            : mapper.Map<ActivityDto>(activity);
    }

    public async Task<(ActivityDto activityDto, int createdActivityId)> CreateAsync(int moduleId, ActivityCreateDto activityCreateDto)
    {
        EnsureModuleExists(moduleId);
        EnsureNotNull(activityCreateDto, "Activity data is null.");
        EnsureActivityStartsBeforeEndDate(activityCreateDto.StartsAt, activityCreateDto.EndsAt);

        var module = await unitOfWork.ModuleRepository.GetModuleByIdAsync(moduleId, false, false)
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
        EnsureModuleExists(moduleId);
        EnsureNotNull(activityEditDto, "Activity data is null.");
        EnsureActivityStartsBeforeEndDate(activityEditDto.StartsAt, activityEditDto.EndsAt);
        var activity = await unitOfWork.ActivityRepository.GetActivityByIdAsync(activity => activity.Id == id && activity.ModuleId == moduleId, true);

        var module = await unitOfWork.ModuleRepository.GetModuleByIdAsync(moduleId, false, false);
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
        EnsureModuleExists(moduleId);
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

    private void EnsureModuleExists(int moduleId)
    {
        if (moduleId == 0 || !unitOfWork.ActivityRepository.FindByCondition(activity => activity.ModuleId == moduleId).Any())
            throw new NotFoundException($"Module with id '{moduleId}' not found.");
    }

    private static void EnsureNotNull<T>(T obj, string message)
    {
        if (obj == null)
            throw new BadRequestException(message);
    }

    private static void EnsureActivityWithinModule(DateTime startsAt, DateTime endsAt, Module module)
    {
        if (startsAt < module.StartsAt || endsAt > module.EndsAt)
            throw new BadRequestException("Activity must be within module start and end time.");
    }

    private static void EnsureActivityStartsBeforeEndDate(DateTime startsAt, DateTime endsAt)
    {
        if (startsAt >= endsAt)
            throw new BadRequestException("Start date must be before end date.");
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
