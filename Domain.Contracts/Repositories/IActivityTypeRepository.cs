using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;
public interface IActivityTypeRepository : IRepositoryBase<ActivityType>, IInternalRepositoryBase<ActivityType>
{
    Task<IEnumerable<ActivityType>> GetAllAsync(bool trackChanges);
}
