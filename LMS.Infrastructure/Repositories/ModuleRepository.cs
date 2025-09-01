using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using LMS.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace LMS.Infrastructure.Repositories;

public class ModuleRepository(ApplicationDbContext context) : RepositoryBase<Module>(context), IModuleRepository
{

    public async Task<PagedList<Module>> GetModulesAsync(
        int courseId,
        ModuleRequestParams requestParams,
        bool trackChanges = false
        )
    {
        var query = FindByCondition(module => module.CourseId.Equals(courseId), trackChanges);

        if (requestParams.IncludeActivities)
        {
            query = query.Include(module => module.Activities);
        }

        query = ApplyOrdering(query, requestParams);

        return await PagedList<Module>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<Module?> GetModuleByConditionAsync(Expression<Func<Module, bool>> expression, bool includeActivities, bool trackChanges)
    {
        var query = FindByCondition(expression, trackChanges);

        if (includeActivities)
            query = query.Include(module => module.Activities);

        return await query.FirstOrDefaultAsync();
    }



    public async Task<bool?> HasOverlappingAsync(
        int courseId,
        DateTime startsAt,
        DateTime endsAt,
        int? excludeModuleId = null
        )
    {
        var query = Context.Modules.Where(module => module.CourseId == courseId);
        if (query is null) return null;

        if (excludeModuleId is not null)
            query = query.Where(module => module.Id != excludeModuleId);

        return await query.AnyAsync(module => startsAt < module.EndsAt && endsAt > module.StartsAt);
    }

    public async Task<bool> CourseExistAsync(int courseId)
    {
        if (courseId <= 0) return false;
        return await Context.Courses.AnyAsync(course => course.Id == courseId);
    }

    private static IQueryable<Module> ApplyOrdering(IQueryable<Module> modules, RequestParams requestParams)
    {
        if (requestParams.OrderBy == null) return modules.OrderBy(m => m.StartsAt).ThenBy(m => m.Name);

        return requestParams.OrderBy switch
        {
            OrderByParams.NameAsc => modules.OrderBy(m => m.Name),
            OrderByParams.NameDesc => modules.OrderByDescending(t => t.Name),
            OrderByParams.DateAsc => modules.OrderBy(m => m.StartsAt).ThenBy(m => m.Name),
            OrderByParams.DateDesc => modules.OrderByDescending(m => m.StartsAt).ThenBy(m => m.Name),
            _ => modules
        };
    }
}