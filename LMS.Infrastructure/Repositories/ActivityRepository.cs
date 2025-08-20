using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;
public class ActivityRepository(ApplicationDbContext context) : RepositoryBase<Activity>(context), IActivityRepository
{
    public async Task<PagedList<Activity>> GetModuleActivities(int moduleId, RequestParams requestParams, bool trackChanges = false)
    {
        var query = FindByCondition(a => a.ModuleId == moduleId
                                    && (requestParams.SearchTerm == null
                                        // Obs! :DO NOT USE ¨StringComparison.OrdinalIgnoreCase¨ here.
                                        // Using ToLower() for case-insensitive search instead 
                                        || a.Name.ToLower().Contains(requestParams.SearchTerm.ToLower())),
                                    trackChanges);

        query = ApplyOrdering(query, requestParams);

        return await PagedList<Activity>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<bool> AnyOverlappingAsync(int moduleId, DateTime startsAt, DateTime endsAt, int? excludeActivityId = null)
    {
        return await Context.Activities
            .Where(a => a.ModuleId == moduleId && (excludeActivityId == null || a.Id != excludeActivityId))
            .AnyAsync(a => startsAt < a.EndsAt && endsAt > a.StartsAt);
    }

    private static IQueryable<Activity> ApplyOrdering(IQueryable<Activity> activities, RequestParams requestParams)
    {
        if (string.IsNullOrEmpty(requestParams.OrderBy)) return activities;

        return requestParams.OrderBy.ToLower() switch
        {
            "name" => activities.OrderBy(t => t.Name),
            "startdate" => activities.OrderBy(t => t.StartsAt),
            "enddate" => activities.OrderBy(t => t.EndsAt),
            _ => activities
        };
    }
}
