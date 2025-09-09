using LMS.Shared.Common;
using LMS.Shared.DTOs.UserDtos;

namespace Service.Contracts;

public interface IUserService
{
    Task<(IEnumerable<UserBasicDto> users, MetaData metaData)> GetAllAsync(RequestParams parameter,
                                                                            bool includeDocuments = false,
                                                                            bool trackChanges = false);
    Task<(IEnumerable<UserBasicDto> users, MetaData metaData)> GetStudentsByCourseIdAsync(RequestParams parameter,
                                                                                            int courseId,
                                                                                            bool includeDocuments = false,
                                                                                            bool trackChanges = false);
    Task<UserBasicDto> GetByIdAsync(string id, bool includeDocuments = false, bool trackChanges = false);
    Task<UserBasicDto> UpdateAsync(string id, UserUpdateDto activityEditDto);
    Task DeleteAsync(string id);
    Task<UserBasicDto> InviteAsync(UserInviteDto userInviteDto);
}