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
    Task<(Module, ModuleUpdateDto)> GetModuleForPatchAsync(int id);
    Task<ModuleDto> CreateModuleAsync(ModuleCreateDto dto);
    Task UpdateModuleAsync(int id, ModuleUpdateDto dto);
    Task ApplyModulePatchAsync(Module module, ModuleUpdateDto dto);
}