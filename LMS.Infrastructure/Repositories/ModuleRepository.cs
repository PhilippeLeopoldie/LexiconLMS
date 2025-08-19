using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using Microsoft.EntityFrameworkCore;


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

}