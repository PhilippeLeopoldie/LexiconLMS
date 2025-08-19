using LMS.Shared.DTOs.ActivityDtos;
namespace Service.Contracts;
public interface IActivityService
{
    // Task<IEnumerable<ActivityDto>> GetAllAsync(QueryParameters queryParameters);
    Task<ActivityDto> GetByIdAsync(int id);
    Task<ActivityDto> CreateAsync(ActivityCreateDto activityCreateDto);
    Task UpdateAsync(int id, ActivityEditDto activityEditDto);
    Task DeleteAsync(int id);
}
