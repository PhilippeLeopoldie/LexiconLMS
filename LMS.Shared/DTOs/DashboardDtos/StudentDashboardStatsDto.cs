namespace LMS.Shared.DTOs.DashboardDtos;
public record StudentDashboardStatsDto
{
    public string? ModulesCount { get; init; }
    public string? PassedModulesCount { get; init; }
    public string? AssignmentsCount { get; init; }
    public string? UpcomingActivitiesCount { get; init; }
}
