using Domain.Models.Entities;
using LMS.Shared.Common;
using System.Linq.Expressions;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository : IRepositoryBase<Module>, IInternalRepositoryBase<Module>
{
    Task<Module?> GetModuleByConditionAsync(Expression<Func<Module, bool>> expression, bool includeActivities, bool trackChanges);
    Task<PagedList<Module>> GetModulesAsync(
        int courseId,
        ModuleRequestParams requestParams,
        bool trackChanges = false
        );
    //Task<Module?> GetModuleByNameAsync(string name, bool trackChanges);
    Task<bool?> HasOverlappingAsync(
        int courseId,
        DateTime startsAt,
        DateTime endsAt,
        int? excludeModuleId = null
        );
    Task<bool> CourseExistAsync(int courseId);
    Task<int> GetModulesCountAsync(int? courseId);
    Task<int> GetPassedModulesCountAsync(int? courseId);
}