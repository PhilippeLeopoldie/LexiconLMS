using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.DTOs.UserDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : RepositoryBase<ApplicationUser>(context), IUserRepository
{

    private readonly IMapper mapper;

    public UserRepository(ApplicationDbContext context, IMapper mapper)
        : this(context)
    {
        this.mapper = mapper;
    }


    public async Task AddUserAsync(ApplicationUser user, bool trackChanges = false)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        Create(user);
        
    }


    public async Task DeleteUserAsync(string id, bool trackChanges = false)
    {
        var user = await FindByCondition(u => u.Id == id, trackChanges)
        .FirstOrDefaultAsync();

        if (user is null)
            throw new KeyNotFoundException($"User with id {id} was not found.");

        Delete(user);
        
    }

    
    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(bool trackChanges = false)
    {
        return await FindAll(trackChanges)
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }


    public async Task<UserBasicDto?> GetUserByIdAsync(string id, bool trackChanges = false)
    {
        var user = await FindByCondition(u => u.Id == id, trackChanges)
        .FirstOrDefaultAsync();

        return user is null ? null : mapper.Map<UserBasicDto>(user);
    }

    public async Task UpdateUserAsync(ApplicationUser user, bool trackChanges = false)
    {
        var existingUser = await FindByCondition(u => u.Id == user.Id, trackChanges)
        .FirstOrDefaultAsync();

        if (existingUser is null)
            throw new KeyNotFoundException($"User with id {user.Id} was not found.");

        // Update the existing user with new values
        existingUser.UserName = user.UserName;
        existingUser.Email = user.Email;

        Update(existingUser);
        await Context.SaveChangesAsync();
    }

}
