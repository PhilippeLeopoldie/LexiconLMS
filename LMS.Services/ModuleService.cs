using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using Service.Contracts;

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
        int courseId,
        ModuleRequestParams requestParams,
        bool trackChanges = false
        )
    {
        var pagedList = await _uow.ModuleRepository.GetModulesAsync(courseId, requestParams, trackChanges);
        var ModulesDto = _mapper.Map<IEnumerable<ModuleDto>>(pagedList.Items);
        return (ModulesDto, pagedList.MetaData);
    }

    public async Task<ModuleDto> GetModuleByIdAsync(int courseId,int id, bool includeActivities)
    {
        var module = await GetModuleByIdOrThrowExceptionAsync(courseId, id, includeActivities, trackChanges: false);
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task<ModuleDto> GetModuleByNameAsync(int courseId, string name, bool includeActivities)
    {
        var module = await _uow.ModuleRepository.GetModuleByConditionAsync(
            module => module.Name == name && module.CourseId == courseId,
            includeActivities,
            trackChanges: false
            );
        if (module is null) throw new ModuleNotFoundException(name);
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task UpdateModuleAsync(int courseId, int id, ModuleUpdateDto dto)
    {
        //if (id != dto.Id) throw new InvalidEntryBadRequestException(id);
        await HasAnyOverlapping(courseId, dto, id);
        var module = await GetModuleByIdOrThrowExceptionAsync(courseId, id, includeActivities: false, trackChanges: true);
        _mapper.Map(dto, module);
        await _uow.CompleteAsync();
    }

    

    public async Task<(Module, ModuleUpdateDto)> GetModuleForPatchAsync(int courseId, int id)
    {
        var module = await GetModuleByIdOrThrowExceptionAsync(courseId, id, includeActivities: true, trackChanges: true);
        var dto = _mapper.Map<ModuleUpdateDto>(module);
        return (module, dto);
    }

    public async Task ApplyModulePatchAsync(Module module, ModuleUpdateDto dto)
    {
        _mapper.Map(dto, module);
        await _uow.CompleteAsync();
    }

    public async Task<ModuleDto> CreateModuleAsync(int courseId, ModuleCreateDto dto)
    {
        await HasAnyOverlapping(courseId, dto);
        var module = _mapper.Map<Module>(dto);
        module.Id = courseId;
        _uow.ModuleRepository.Create(module);
        await _uow.CompleteAsync();
        return _mapper.Map<ModuleDto>(module);
    }

    public async Task DeleteModuleAsync(int courseId, int id)
    {
        var module = await GetModuleByIdOrThrowExceptionAsync(courseId, id, includeActivities: true, trackChanges: true);
        _uow.ModuleRepository.Delete(module);
        await _uow.CompleteAsync();
    }

    private async Task<Module> GetModuleByIdOrThrowExceptionAsync(int courseId, int id, bool includeActivities, bool trackChanges)
    {
        var module = await _uow.ModuleRepository.GetModuleByConditionAsync(
            module => module.Id == id && module.CourseId == courseId,
            includeActivities,
            trackChanges
            );
        return module ?? throw new ModuleNotFoundException(id); 
    }

    private async Task HasAnyOverlapping(int courseId, ModuleForManipulationDto dto, int? id = null)
    {
        var hasAnyOverlapping = await _uow.ModuleRepository.HasOverlappingAsync(courseId, dto.StartsAt, dto.EndsAt, id)
            ?? throw new NotFoundException($"There is no module with CourseId: {courseId}");
        if (hasAnyOverlapping)
        {
            throw new ModuleOverlappingException($"{dto.StartsAt:yyyy-MM-dd HH:mm} - {dto.EndsAt:yyyy-MM-dd HH:mm}");
        }
    }

}
