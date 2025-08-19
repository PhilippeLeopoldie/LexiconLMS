using Domain.Models.Entities;
using LMS.Shared.Common;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetModuleByIdAsync(int id, bool includeActivities, bool trackChanges);
    Task<PagedList<Module>> GetModulesAsync(
        ModuleRequestParams requestParams,
        int courseId,
        bool sortByName = false,
        bool trackChanges = false
        );
}