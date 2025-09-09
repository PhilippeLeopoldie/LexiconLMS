using LMS.Shared.DTOs.DashboardDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IServiceManager serviceManager) : ControllerBase
{
    [HttpGet("teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<TeacherDashboardStatsDto>> GetTeacherStats()
    {
        var stats = await serviceManager.DashboardService.GetTeacherStatsAsync(User);
        return Ok(stats);
    }

    [HttpGet("student")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<TeacherDashboardStatsDto>> GetStudentStats()
    {
        var stats = await serviceManager.DashboardService.GetStudentStatsAsync(User);
        return Ok(stats);
    }
}
