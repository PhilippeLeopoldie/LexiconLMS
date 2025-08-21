using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Service.Contracts;
using System.Text.Json;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController(IServiceManager serviceManager) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesAsync([FromQuery] RequestParams requestParams)
    {
        var (courses, metaData) = await serviceManager.CourseService.GetAllCoursesAsync(requestParams);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(courses);
    }

    [HttpGet("{userId:string}")]
    [Authorize(Roles = "Teacher, Student")]  // NOTE: Not really needed
    public async Task<ActionResult<CourseDto>> GetCourseForStudentAsync(
        string userId, 
        [FromQuery] bool includeModules = false,
        [FromQuery] bool includeActivities = false, 
        [FromQuery] RequestParams requestParams = null!
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(userId, nameof(userId));

        var (course, metaData) = await serviceManager.CourseService.GetCourseForUserAsync(userId, includeModules, includeActivities, requestParams);
        if (course == null)
            return NotFound($"No course found for user {userId}.");
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(course);
    }

    [HttpGet("{courseId:int}")]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<ActionResult<CourseDto>> GetCourseByIdAsync(
        int courseId, 
        [FromQuery] bool includeModules = false, 
        [FromQuery] bool includeActivities = false,
        [FromQuery] RequestParams requestParams = null!)
    {
        var course = await serviceManager.CourseService.GetCourseByIdAsync(courseId, includeModules, includeActivities, requestParams);
        if (course == null)
            return NotFound($"Course with ID {courseId} not found.");
        return Ok(course);
    }
}
