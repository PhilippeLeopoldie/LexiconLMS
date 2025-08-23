using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;
public class ActivityTypeRepository(ApplicationDbContext context) : RepositoryBase<ActivityType>(context), IActivityTypeRepository
{
    public async Task<IEnumerable<ActivityType>> GetAllAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .OrderBy(at => at.Name)
            .ToListAsync();
    }

    public async Task<int?> GetAssignmentTypeIdAsync()
    {
        var assignmentType = await FindByCondition(at => at.Name == "Inlämningsuppgift").FirstOrDefaultAsync();
        return assignmentType?.Id;
    }
}
