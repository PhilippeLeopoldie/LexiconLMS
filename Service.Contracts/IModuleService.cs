using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;

namespace Services.Contracts;

public interface IModuleService
{
    Task DeleteModuleAsync(int courseId, int id);
    Task<(IEnumerable<ModuleDto> moduleDtos, MetaData metaData)> GetAllModulesAsync(int courseId, ModuleRequestParams requestParams, bool trackChanges = false);
    Task<ModuleDto> GetModuleByIdAsync(int courseId, int id, bool includeActivities);
    Task<ModuleDto> GetModuleByNameAsync(int courseId, string name, bool includeActivities);
    Task<(Module, ModuleUpdateDto)> GetModuleForPatchAsync(int courseId, int id);
    Task<ModuleDto> CreateModuleAsync(ModuleCreateDto dto);
    Task UpdateModuleAsync(int courseId, int id, ModuleUpdateDto dto);
    Task ApplyModulePatchAsync(Module module, ModuleUpdateDto dto);
}