using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.Common;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;

namespace LMS.Services
{
    public class UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailSender<ApplicationUser> emailSender) : ServiceBase, IUserService
    {
        public async Task<(IEnumerable<UserBasicDto> users, MetaData metaData)> GetAllAsync(RequestParams parameter, bool includeDocuments = false, bool trackChanges = false)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

            PagedList<ApplicationUser> pagedUsers = await unitOfWork.UserRepository.GetAllUsersAsync(parameter, includeDocuments, trackChanges);

            var usersDto = mapper.Map<IEnumerable<UserBasicDto>>(pagedUsers.Items);

            return (usersDto, pagedUsers.MetaData);
        }

        public async Task<(IEnumerable<UserBasicDto> users, MetaData metaData)> GetStudentsByCourseIdAsync(RequestParams parameter, int courseId, bool includeDocuments = false, bool trackChanges = false)
        {
            ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
            var pagedUsers = await unitOfWork.UserRepository.GetStudentsByCourseAsync(parameter, courseId, includeDocuments, trackChanges);

            var usersDto = mapper.Map<IEnumerable<UserBasicDto>>(pagedUsers.Items);

            return (usersDto, pagedUsers.MetaData);
        }

        public async Task<UserBasicDto> GetByIdAsync(string id, bool includeDocuments = false, bool trackChanges = false)
        {
            var user = await userManager.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"User with ID {id} was not found");
            return mapper.Map<UserBasicDto>(user);
        }

        public async Task<UserBasicDto> InviteAsync(UserInviteDto userInviteDto)
        {
            var user = mapper.Map<ApplicationUser>(userInviteDto);
            user.EmailConfirmed = true;

            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(user, userInviteDto.Role.ToString());

            if (userInviteDto.CourseId.HasValue)
            {
                var course = await unitOfWork.CourseRepository.GetCourseByIdAsync(userInviteDto.CourseId.Value, trackChanges: false)
                             ?? throw new KeyNotFoundException($"Course with ID {userInviteDto.CourseId.Value} was not found");

                if (userInviteDto.Role == UserRole.Student)
                    user.CourseId = userInviteDto.CourseId;
                else if (userInviteDto.Role == UserRole.Teacher)
                {
                    user.CourseId = null;
                    user.Courses.Add(course);
                }
            }

            var registrationUrl = $"https://localhost:7224/Account/CompleteRegistration?email={Uri.EscapeDataString(user.Email!)}";

            await emailSender.SendPasswordResetLinkAsync(user, user.Email!, registrationUrl);

            await unitOfWork.CompleteAsync();
            return mapper.Map<UserBasicDto>(user);
        }


        public async Task<UserBasicDto> UpdateAsync(string id, UserUpdateDto userUpdateDto)
        {
            var updatedUser = await userManager.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"User with ID {id} was not found");

            updatedUser.FirstName = userUpdateDto.FirstName;
            updatedUser.LastName = userUpdateDto.LastName;
            updatedUser.PhoneNumber = userUpdateDto.PhoneNumber;
            if (userUpdateDto.CourseId.HasValue)
            {

                if (userUpdateDto.Role == UserRole.Student)
                {
                    updatedUser.CourseId = userUpdateDto.CourseId.Value;
                }
                else
                {
                    updatedUser.CourseId = null;
                }
            }

            if (userUpdateDto.Role.HasValue)
            {
                var currentRoles = await userManager.GetRolesAsync(updatedUser);
                var newRole = userUpdateDto.Role.Value.ToString();

                if (newRole != null && !currentRoles.Contains(newRole))
                {
                    await userManager.RemoveFromRolesAsync(updatedUser, currentRoles);
                    await userManager.AddToRoleAsync(updatedUser, newRole);
                }
            }
            var result = await userManager.UpdateAsync(updatedUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return mapper.Map<UserBasicDto>(updatedUser);
        }

        public async Task DeleteAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id)
                ?? throw new KeyNotFoundException($"User with ID {id} was not found");
            try
            {
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await unitOfWork.CompleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception($"An error occurred while deleting the user: {ex.Message}", ex);
            }
        }
    }
}
