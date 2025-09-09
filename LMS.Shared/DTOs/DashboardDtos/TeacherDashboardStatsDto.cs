namespace LMS.Shared.DTOs.DashboardDtos;
public record TeacherDashboardStatsDto()
{
    public string? CoursesCount { get; init; }
    public string? StudentsCount { get; init; }
    public string? AssignmentsCount { get; init; }
    public string? UpcomingActivitiesCount { get; init; }
}
