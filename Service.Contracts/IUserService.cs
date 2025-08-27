using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.DTOs.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IUserService
{
    Task<UserBasicDto> GetUserByIdAsync(string id, bool trackChanges = false);
    Task<(IEnumerable<UserBasicDto> users, int totalCount)> GetAllUsersAsync(
        string? role = null,
        int pageNumber = 1,
        int pageSize = 10,
        bool trackChanges = false);
    Task AddUserAsync(ApplicationUser user, bool trackChanges = false);
    Task UpdateUserAsync(UserBasicDto user, bool trackChanges = false);
    Task DeleteUserAsync(string id, bool trackChanges = false);
}
