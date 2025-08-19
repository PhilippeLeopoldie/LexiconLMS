using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;

namespace Services.Contracts;

public interface IModuleService
{
    Task DeleteModuleAsync(int id);
    Task<(IEnumerable<ModuleDto> moduleDtos, MetaData metaData)> GetAllModulesAsync(ModuleRequestParams requestParams, int courseId, bool sortByName = false, bool trackChanges = false);
    Task<ModuleDto> GetModuleByIdAsync(int id, bool includeActivities);
    Task<ModuleDto> GetModuleByNameAsync(string name);
    Task<(Module, ModuleUpdateDto)> ModuleToPatchAsync(int id);
    Task<ModuleDto> PostModuleAsync(ModuleCreateDto dto);
    Task PutModuleAsync(int id, ModuleUpdateDto dto);
    Task SavePatchModuleAsync(Module module, ModuleUpdateDto dto);
}