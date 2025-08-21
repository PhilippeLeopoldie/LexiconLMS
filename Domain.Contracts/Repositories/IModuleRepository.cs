using Domain.Models.Entities;
using LMS.Shared.Common;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository : IRepositoryBase<Module>, IInternalRepositoryBase<Module>
{
    Task<Module?> GetModuleByIdAsync(int id, bool includeActivities, bool trackChanges);
    Task<PagedList<Module>> GetModulesAsync(
        int courseId,
        ModuleRequestParams requestParams,
        bool trackChanges = false
        );
    Task<Module?> GetModuleByNameAsync(string name, bool trackChanges);
    Task<bool> HasOverlappingAsync(
        int courseId,
        DateTime startsAt,
        DateTime endsAt,
        int? excludeModuleId = null
        );
}