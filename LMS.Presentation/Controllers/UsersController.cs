using LMS.Shared.Common;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IServiceManager serviceManager) : ControllerBase
{
    private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    private bool IsStudent() => User.IsInRole("Student");

    [HttpGet]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Get all users", Description = "Get all users with pagination (Teacher only)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Users retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult> GetAllUsersAsync([FromQuery] RequestParams requestParameter, bool includeDocuments = false)
    {
        var (users, metaData) = await serviceManager.UserService.GetAllAsync(requestParameter, includeDocuments);

        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metaData));
        return Ok(users);
    }

    //[HttpGet("teachers")]
    //[Authorize(Roles = "Teacher")]
    //[SwaggerOperation(Summary = "Get all teachers", Description = "Get all teachers with optional course filter (Teacher only)")]
    //[SwaggerResponse(StatusCodes.Status200OK, "Teachers retrieved successfully")]
    //[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    //[SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    //public async Task<ActionResult> GetTeachersAsync([FromQuery] RequestParams requestParameter, int? courseId, bool includeDocuments = false)
    //{
    //    var (teachers, metaData) = await serviceManager.UserService.GetTeachersAsync(requestParameter, courseId, includeDocuments);

    //    Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metaData));
    //    return Ok(teachers);
    //}

    [HttpGet("students")]
    [Authorize(Roles = "Teacher,Student")]
    [SwaggerOperation(Summary = "Get students", Description = "Get students by course (Teachers see all, Students see only their course)")]
    [SwaggerResponse(StatusCodes.Status200OK, "Students retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult> GetStudentsAsync([FromQuery] RequestParams requestParameter, int courseId, bool includeDocuments = false)
    {
        if (IsStudent())
        {
            var currentUser = await serviceManager.UserService.GetByIdAsync(GetCurrentUserId());
            if (currentUser.CourseId != courseId)
                return Forbid("Students can only view students from their own course");
        }

        var (students, metaData) = await serviceManager.UserService.GetStudentsByCourseIdAsync(requestParameter, courseId, includeDocuments);

        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metaData));
        return Ok(students);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Teacher,Student")]
    [SwaggerOperation(Summary = "Get user by id", Description = "Get user by id (Teachers see all, Students see only themselves)")]
    [SwaggerResponse(StatusCodes.Status200OK, "User retrieved successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<UserBasicDto>> GetUserById(string id, bool includeDocuments = false)
    {
        if (IsStudent() && GetCurrentUserId() != id)
            return Forbid("Students can only view their own profile");

        try
        {
            var user = await serviceManager.UserService.GetByIdAsync(id, includeDocuments);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("invite")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Invite new user", Description = "Invite a new user (Teacher only)")]
    [SwaggerResponse(StatusCodes.Status201Created, "User invited successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid invitation data")]
    public async Task<IActionResult> InviteUser([FromBody] UserInviteDto userInviteDto)
    {
        var user = await serviceManager.UserService.InviteAsync(userInviteDto);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update user", Description = "Update user (Teachers can update all, Students can update only themselves)")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (IsStudent())
        {
            if (GetCurrentUserId() != id)
                return Forbid("Students can only update their own profile");
            if (userDto.Role != null || userDto.CourseId != null)
                return Forbid("Students cannot change their role or course");
        }

        await serviceManager.UserService.UpdateAsync(id, userDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Delete user", Description = "Delete user (Teacher only)")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User deleted successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await serviceManager.UserService.DeleteAsync(id);
        return NoContent();

    }

    [HttpPost("{id}/assign-role")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Assign role to user", Description = "Assign role to user (Teacher only)")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Role assigned successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid role data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> AssignRole(string id, [FromBody] UserRole role)
    {
        var user = await serviceManager.UserService.GetByIdAsync(id);
        var userDto = new UserUpdateDto(user.UserName,
                                        user.Email,
                                        null, // Password
                                        user.FirstName,
                                        user.LastName,
                                        user.PhoneNumber,
                                        role,
                                        user.CourseId
                                    );

        await serviceManager.UserService.UpdateAsync(id, userDto);
        return NoContent();
    }

    [HttpPost("{id}/assign-course")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Assign course to student", Description = "Assign course to student (Teacher only)")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Course assigned successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<IActionResult> AssignCourse(string id, [FromBody] int courseId)
    {
        var user = await serviceManager.UserService.GetByIdAsync(id);
        var userDto = new UserUpdateDto(user.UserName,
                                        user.Email,
                                        null, // Password
                                        user.FirstName,
                                        user.LastName,
                                        user.PhoneNumber,
                                        user.Role,
                                        courseId
                                    );
        await serviceManager.UserService.UpdateAsync(id, userDto);
        return NoContent();
    }
}