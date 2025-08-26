using Domain.Models.Entities;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IServiceManager serviceManager) : ControllerBase
{

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "List all users", Description = "List all registered users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Users retrieved successfully", typeof(IEnumerable<UserBasicDto>))]
    public async Task<ActionResult<PaginatedResponse<UserBasicDto>>> GetUsersAsync(
        [FromQuery] string? role = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest("Page number and page size must be greater than 0");

        var (users, totalCount) = await serviceManager.UserService.GetAllUsersAsync(
            role,
            pageNumber,
            pageSize,
            trackChanges: false);

        if (users == null)
        {
            return NotFound();
        }

        var response = new PaginatedResponse<UserBasicDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return Ok(response);
    }



    [HttpGet("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Get user by id", Description = "Retrieve a single user by it's id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User retrieved successfully", typeof(UserBasicDto))]
    public async Task<ActionResult<UserBasicDto>> GetUserById(string id)
    {
        var user = await serviceManager.UserService.GetUserByIdAsync(id, trackChanges: false);
        if (user == null)
            return NotFound();
        return Ok(user);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Delete user", Description = "Delete an existing user")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User deleted successfully")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            await serviceManager.UserService.DeleteUserAsync(id, trackChanges: false);

        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Update user", Description = "Update an existing user")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User updated successfully")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserBasicDto userDto)
    {
        if (id != userDto.Id)
            return BadRequest("User ID mismatch");

        try
        {
            await serviceManager.UserService.UpdateUserAsync(userDto, trackChanges: true);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }


    [HttpPost]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Create user", Description = "Create a new user")]
    [SwaggerResponse(StatusCodes.Status201Created, "User created successfully", typeof(UserBasicDto))]
    public async Task<ActionResult<UserBasicDto>> CreateUser([FromBody] UserCreationDto userDto)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email
            };

            await serviceManager.UserService.AddUserAsync(user, trackChanges: false);

            var createdUser = await serviceManager.UserService.GetUserByIdAsync(user.Id, trackChanges: false);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, createdUser);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


}

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
