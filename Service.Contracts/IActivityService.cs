using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
namespace Service.Contracts;
public interface IActivityService
{
    Task<(IEnumerable<ActivityDto> activities, MetaData metaData)> GetAllAsync(int moduleId, RequestParams parameter, bool trackChanges = false);
    Task<ActivityDto> GetByIdAsync(int moduleId, int id, bool trackChanges = false);
    Task<(ActivityDto activityDto, int createdActivityId)> CreateAsync(int moduleId, ActivityCreateDto activityCreateDto);
    Task UpdateAsync(int moduleId, int id, ActivityEditDto activityEditDto);
    Task DeleteAsync(int moduleId, int id);
    Task<IEnumerable<AssignmentDto>> GetStudentAssignmentsAsync(string studentUserId);
}
