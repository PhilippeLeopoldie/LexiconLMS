using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace LMS.Infrastructure.Repositories;

public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
{
    public ModuleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Module?> GetModulesAsync(int courseId, bool trackChanges = false)
    {
        return await FindByCondition(module => module.CourseId.Equals(courseId), trackChanges).FirstOrDefaultAsync();
    }

    public async Task<Module> GetModuleByIdAsync(int id, bool includeActivities, bool trackChanges)
    {
        var query = FindByCondition(module => module.Id.Equals(id), trackChanges);

        if (includeActivities) query = query.Include(module => module.Activities);
        return await query.FirstOrDefaultAsync();
    }

}