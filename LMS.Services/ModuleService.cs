using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;

namespace LMS.Services;

public class ModuleService
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
        var pagedList = await _uow.ModuleRepository.GetModulesAsync(requestParams, courseId ,sortByName, trackChanges);
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



    private async Task<Module> GetModuleByIdOrThrowExceptionAsync(int id, bool includeActivities, bool trackChanges)
    {
        var module = await _uow.ModuleRepository.GetModuleByIdAsync(id, includeActivities, trackChanges);
        if (module is null) throw new ModuleNotFoundException(id);
        return module;
    }

}
