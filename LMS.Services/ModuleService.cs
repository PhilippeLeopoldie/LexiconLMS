using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ModuleDtos;
using Service.Contracts;

namespace LMS.Services;

public class ModuleService : ServiceBase, IModuleService
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
        var modulesDto = _mapper.Map<IEnumerable<ModuleDto>>(pagedList.Items);
        return (modulesDto, pagedList.MetaData);
    }

    public async Task<ModuleDto> GetModuleByIdAsync(int courseId, int id, bool includeActivities)
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
        ValidateDateRange(dto.StartsAt, dto.EndsAt);
        var module = await GetModuleByIdOrThrowExceptionAsync(courseId, id, includeActivities: false, trackChanges: true);
        await EnsureNoOverlapAsync(courseId, dto, id);
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
        ValidateDateRange(dto.StartsAt, dto.EndsAt);
        await EnsureNoOverlapAsync(courseId, dto);
        var module = _mapper.Map<Module>(dto);
        module.CourseId = courseId;
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

    private async Task EnsureNoOverlapAsync(int courseId, ModuleForManipulationDto dto, int? id = null)
    {
        await EnsureModuleHasNoOverLapAsync(courseId, dto, id);
        await EnsureModuleWithinCourse(dto.StartsAt, dto.EndsAt, courseId);
    }

    private async Task EnsureModuleHasNoOverLapAsync(int courseId, ModuleForManipulationDto dto, int? id)
    {
        var hasAnyModuleOverlapping = await _uow.ModuleRepository.HasOverlappingAsync(courseId, dto.StartsAt, dto.EndsAt, id);

        if (hasAnyModuleOverlapping is null)
            throw new CourseNotFoundException(courseId);

        if (hasAnyModuleOverlapping.Value)
            throw new ModuleOverlappingException($"{dto.StartsAt:yyyy-MM-dd HH:mm} - {dto.EndsAt:yyyy-MM-dd HH:mm}");
    }

    private async Task EnsureModuleWithinCourse(DateTime startsAt, DateTime endsAt, int courseId)
    {
        var course = await _uow.CourseRepository.GetCourseByIdAsync(courseId, false)
            ?? throw new NotFoundException($"The Course with id: {courseId} is not found!");

        if (startsAt < course.Starts || endsAt > course.Ends)
            throw new BadRequestException("Module must be within course start and end time.");
    }

}
