using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.DTOs.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Repositories;

public interface IUserRepository : IRepositoryBase<ApplicationUser>, IInternalRepositoryBase<ApplicationUser>
{
    Task<UserBasicDto?> GetUserByIdAsync(string id, bool trackChanges = false);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(bool trackChanges = false);
    Task AddUserAsync(ApplicationUser user, bool trackChanges = false);
    Task UpdateUserAsync(ApplicationUser user, bool trackChanges = false);
    Task DeleteUserAsync(string id, bool trackChanges = false);
}
