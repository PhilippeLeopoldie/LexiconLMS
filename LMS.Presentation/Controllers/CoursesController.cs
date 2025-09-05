using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController(IServiceManager serviceManager) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "List all courses", Description = "List all registered courses")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully", typeof(IEnumerable<CourseDto>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request parameters")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesAsync(
        [FromQuery] UserRole? includeUsers = null,
        [FromQuery] bool includeModules = false,
        [FromQuery] bool includeActivities = false,
        [FromQuery] RequestParams requestParams = null!
    )
    {
        var (courses, metaData) = await serviceManager.CourseService.GetAllCoursesAsync(
            includeUsers,
            includeModules,
            includeActivities,
            requestParams);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(courses);
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Teacher, Student")]  // NOTE: Not really needed
    [SwaggerOperation(Summary = "Get course by student", Description = "Retrieve course info for a specific student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course retrieved successfully", typeof(CourseDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Student not found")]
    public async Task<ActionResult<CourseDto>> GetCourseForStudentAsync(
        string userId, 
        [FromQuery] bool includeModules = false,
        [FromQuery] bool includeActivities = false,
        [FromQuery] RequestParams requestParams = null!
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(userId, nameof(userId));
        var (course, metaData) = await serviceManager.CourseService.GetCourseForUserAsync(
            userId,
            includeModules,
            includeActivities,
            requestParams
            );
        if (course == null)
            return NotFound($"No course found for user {userId}.");
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(course);
    }

    [HttpGet("{courseId:int}")]
    [ActionName(nameof(GetCourseByIdAsync))]
    [Authorize(Roles = "Teacher, Student")]
    [SwaggerOperation(Summary = "Get course by id", Description = "Retrieve a single course by it's id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course retrieved successfully", typeof(CourseDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Id not found")]
    public async Task<ActionResult<CourseDto>> GetCourseByIdAsync(
        int courseId,
        [FromQuery] UserRole? includeUsers = null,
        [FromQuery] bool includeModules = false, 
        [FromQuery] bool includeActivities = false,
        [FromQuery] RequestParams requestParams = null!)
    {
        var course = await serviceManager.CourseService.GetCourseByIdAsync(
            courseId,
            includeUsers,
            includeModules,
            includeActivities,
            requestParams);
        if (course == null)
            return NotFound($"Course with ID {courseId} not found.");
        return Ok(course);
    }

    [HttpPost]
    [Authorize (Roles = "Teacher")]
    [SwaggerOperation(Summary = "Create courses", Description = "Create a new course")]
    [SwaggerResponse(StatusCodes.Status201Created, "Course created successfully", typeof(CourseDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<ActionResult<CourseDto>> CreateCourseAsync([FromBody] CourseForCreationDto courseDto)
    {
        if (courseDto == null)
            return BadRequest("Course data is null.");

        var createdCourse = await serviceManager.CourseService.CreateCourseAsync(courseDto);
        return CreatedAtAction(nameof(GetCourseByIdAsync), new { courseId = createdCourse.createdCourseId }, createdCourse);
    }


    [HttpPost("{courseId:int}/add-student")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Add student ", Description = "Add student to course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Student added successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid  data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Student already in the course")]
    public async Task<IActionResult> AddStudentToCourse(int courseId, [FromQuery] string userId )
    {
        await serviceManager.CourseService.AddStudentToCourseAsync(userId, courseId);
        return Ok();
    }

    [HttpPost("{courseId:int}/add-teacher")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(Summary = "Add teacher ", Description = "Add teacher to course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Teacher added successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid  data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Teacher already in the course")]
    public async Task<IActionResult> AddTeacherToCourse(int courseId, [FromQuery] string userId)
    {
        await serviceManager.CourseService.AddTeacherToCourseAsync(userId, courseId);
        return Ok();
    }


    [HttpPut("{courseId:int}")]
    [Authorize (Roles = "Teacher")]
    [SwaggerOperation(Summary = "Update course", Description = "Update an existing course")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Course updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<IActionResult> UpdateCourseAsync(int courseId, [FromBody] CourseForModificationDto courseDto)
    {
        if (courseDto == null)
            return BadRequest("Course data is null.");

        await serviceManager.CourseService.UpdateCourseAsync(courseId, courseDto);
        return NoContent();
    }

    [HttpDelete("{courseId:int}")]
    [Authorize (Roles = "Teacher")]
    [SwaggerOperation(Summary = "Delete course", Description = "Delete an existing course")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Course deleted successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid course data")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authorized")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<IActionResult> DeleteCourseAsync(int courseId)
    {
        await serviceManager.CourseService.DeleteCourseAsync(courseId);
        return NoContent();
    }
}
