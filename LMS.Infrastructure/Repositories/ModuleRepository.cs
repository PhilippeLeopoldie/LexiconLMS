using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace LMS.Infrastructure.Repositories;

public class ModuleRepository(ApplicationDbContext context) : RepositoryBase<Module>(context), IModuleRepository
{

    public async Task<PagedList<Module>> GetModulesAsync(
        ModuleRequestParams requestParams,
        int courseId,
        bool sortByName =false,
        bool trackChanges = false
        )
    {
        var query = FindByCondition(module => module.CourseId.Equals(courseId), trackChanges);

        if(requestParams.IncludeActivities)
            query = query.Include(module => module.Activities);

        if(sortByName)
            query = query.OrderBy(module => module.Name);

        return await PagedList<Module>.CreateAsync(query, requestParams.Page, requestParams.PageSize);
    }

    public async Task<Module?> GetModuleByIdAsync(int id, bool includeActivities, bool trackChanges)
    {
        var query = FindByCondition(module => module.Id.Equals(id), trackChanges);

        if (includeActivities) 
            query = query.Include(module => module.Activities);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<Module?> GetModuleByNameAsync(string name, bool trackChanges)
    {
        return await FindByCondition(module => string.Equals(module.Name, name), trackChanges)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasOverlappingAsync(
        int courseId,
        DateTime startsAt,
        DateTime endsAt,
        int? excludeModuleId = null
        )
    {
        var query = Context.Modules.Where(module => module.CourseId == courseId);

        if(excludeModuleId is not null)
            query = query.Where(module => module.Id != excludeModuleId);

        return await query.AnyAsync(module => startsAt < module.EndsAt && endsAt > module.StartsAt);
    }
}