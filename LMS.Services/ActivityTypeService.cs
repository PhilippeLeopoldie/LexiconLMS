using AutoMapper;
using Domain.Contracts.Repositories;
using LMS.Shared.DTOs.ActivityDtos;
using Service.Contracts;

namespace LMS.Services;
public class ActivityTypeService(IUnitOfWork unitOfWork, IMapper mapper) : IActivityTypeService
{
    public async Task<IEnumerable<ActivityTypeDto>> GetAllAsync(bool trackChanges = false)
    {
        var activityTypes = await unitOfWork.ActivityTypeRepository.GetAllAsync(trackChanges);
        return mapper.Map<IEnumerable<ActivityTypeDto>>(activityTypes);
    }
}
