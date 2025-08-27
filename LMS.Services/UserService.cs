using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager) : IUserService
    {

        public async Task AddUserAsync(ApplicationUser user, bool trackChanges = false)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            }

            //await unitOfWork.UserRepository.AddUserAsync(user, trackChanges);
            await unitOfWork.CompleteAsync();
        }


        public async Task DeleteUserAsync(string id, bool trackChanges = false)
        {
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(id, trackChanges);

            if (user == null)
                throw new InvalidOperationException($"User with ID {id} was not found");

            await unitOfWork.UserRepository.DeleteUserAsync(id, trackChanges);
            await unitOfWork.CompleteAsync();
        }



        public async Task<(IEnumerable<UserBasicDto> users, int totalCount)> GetAllUsersAsync(string? role = null, int pageNumber = 1, int pageSize = 10, bool trackChanges = false)
        {
            var query = unitOfWork.UserRepository.FindAll();

            // Apply role filtering if specified
            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await userManager.GetUsersInRoleAsync(role);
                var userIds = usersInRole.Select(u => u.Id);
                query = query.Where(u => userIds.Contains(u.Id));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<UserBasicDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return (users, totalCount);
        }


        public async Task<UserBasicDto?> GetUserByIdAsync(string id, bool trackChanges = false)
        {
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(id, trackChanges);

            return user == null ? null : mapper.Map<UserBasicDto>(user);
        }


        public async Task UpdateUserAsync(UserBasicDto userDto, bool trackChanges = false)
        {
            var existingUser = await unitOfWork.UserRepository.FindByCondition(u => u.Id.Equals( userDto.Id), trackChanges)
                .FirstOrDefaultAsync();

            if (existingUser == null)
                throw new InvalidOperationException($"User with ID {userDto.Id} was not found");

            // Map the DTO properties to the existing entity
            existingUser.UserName = userDto.UserName;
            existingUser.Email = userDto.Email;

            await unitOfWork.UserRepository.UpdateUserAsync(existingUser, trackChanges);
            await unitOfWork.CompleteAsync();
        }


    }

    
}
