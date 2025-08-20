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
}
