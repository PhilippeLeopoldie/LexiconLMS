using AutoMapper;
using Domain.Contracts.Repositories;
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

    

}
