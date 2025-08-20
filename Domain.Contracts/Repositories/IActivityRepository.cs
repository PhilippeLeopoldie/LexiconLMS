using Domain.Models.Entities;
using LMS.Shared.Common;

namespace Domain.Contracts.Repositories;
public interface IActivityRepository : IRepositoryBase<Activity>, IInternalRepositoryBase<Activity>
{
    Task<PagedList<Activity>> GetModuleActivities(int moduleId, RequestParams requestParams, bool trackChanges);
    Task<bool> AnyOverlappingAsync(int moduleId, DateTime startsAt, DateTime endsAt, int? excludeActivityId = null);
}
