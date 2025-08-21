using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using Services.Contracts;

namespace LMS.Services;

public class ModuleService : IModuleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ModuleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _uow = unitOfWork;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ModuleDto> moduleDtos, MetaData metaData)> GetAllModulesAsync(
        ModuleRequestParams requestParams,
        int courseId,
        bool sortByName = false,
        bool trackChanges = false
        )
    {
        var pagedList = await _uow.ModuleRepository.GetModulesAsync(requestParams, courseId, sortByName, trackChanges);
        var ModulesDto = _mapper.Map<IEnumerable<ModuleDto>>(pagedList.Items);
        return (ModulesDto, pagedList.MetaData);
    }

    public async Task<ModuleDto> GetModuleByIdAsync(int id, bool includeActivities)
    {
        var module = await GetModuleByIdOrThrowExceptionAsync(id, includeActivities, trackChanges: false);
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task<ModuleDto> GetModuleByNameAsync(string name)
    {
        var module = await _uow.ModuleRepository.GetModuleByNameAsync(name, trackChanges: false);
        if (module is null) throw new ModuleNotFoundException(name);
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task UpdateModuleAsync(int id, ModuleUpdateDto dto)
    {
        if (id != dto.Id) throw new InvalidEntryBadRequestException(id);
        var module = await GetModuleByIdOrThrowExceptionAsync(id, includeActivities: false, trackChanges: true);
        _mapper.Map(dto, module);
        await _uow.CompleteAsync();
    }

    public async Task<(Module, ModuleUpdateDto)> GetModuleForPatchAsync(int id)
    {
        var module = await GetModuleByIdOrThrowExceptionAsync(id, includeActivities: true, trackChanges: true);
        var dto = _mapper.Map<ModuleUpdateDto>(module);
        return (module, dto);
    }

    public async Task ApplyModulePatchAsync(Module module, ModuleUpdateDto dto)
    {
        _mapper.Map(dto, module);
        await _uow.CompleteAsync();
    }

    public async Task<ModuleDto> CreateModuleAsync(ModuleCreateDto dto)
    {
        var hasAnyOverlapping = await _uow.ModuleRepository.HasOverlappingAsync(dto.CourseId, dto.StartsAt, dto.EndsAt);

        if (hasAnyOverlapping) 
        {
            throw new ModuleOverlappingException($"{dto.StartsAt:G} - {dto.EndsAt:G}");
        }
       
        var module = _mapper.Map<Module>(dto);
        _uow.ModuleRepository.Create(module);
        await _uow.CompleteAsync();
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task DeleteModuleAsync(int id)
    {
        var module = await GetModuleByIdOrThrowExceptionAsync(id, includeActivities: true, trackChanges: true);
        _uow.ModuleRepository.Delete(module);
        await _uow.CompleteAsync();
    }

    private async Task<Module> GetModuleByIdOrThrowExceptionAsync(int id, bool includeActivities, bool trackChanges)
    {
        var module = await _uow.ModuleRepository.GetModuleByIdAsync(id, includeActivities, trackChanges);
        if (module is null) throw new ModuleNotFoundException(id);
        return module;
    }

}
