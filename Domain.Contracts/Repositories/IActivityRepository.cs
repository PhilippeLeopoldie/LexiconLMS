using Domain.Models.Entities;
using LMS.Shared.Common;
using System.Linq.Expressions;

namespace Domain.Contracts.Repositories;
public interface IActivityRepository : IRepositoryBase<Activity>, IInternalRepositoryBase<Activity>
{
    Task<PagedList<Activity>> GetModuleActivitiesAsync(int moduleId, RequestParams requestParams, bool trackChanges);
    Task<bool> AnyOverlappingAsync(int moduleId, DateTime startsAt, DateTime endsAt, int? excludeActivityId = null);
    Task<Activity?> GetActivityByIdAsync(Expression<Func<Activity, bool>> expression, bool trackChanges = false);
    Task<IEnumerable<Activity>> GetByCourseIdAndTypeIdAsync(int courseId, int activityTypeId);
}
