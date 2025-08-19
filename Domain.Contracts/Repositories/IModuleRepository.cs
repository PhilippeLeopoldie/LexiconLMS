using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository
{
    Task<Module> GetModuleByIdAsync(int id, bool includeActivities, bool trackChanges);
    Task<Module?> GetModulesAsync(int courseId, bool trackChanges = false);
}