using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.DashboardDtos;
using LMS.Shared.DTOs.ModuleDtos;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Enums;
using LMS.Shared.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LMS.Blazor.Client.Pages.Home;
public partial class StudentHome
{
    private bool isLoading = true;
    private string? errorMessage;
    private StudentDashboardStatsDto? stats = new StudentDashboardStatsDto();
    private IEnumerable<ActivityDto>? upcomingActivities = [];
    private IEnumerable<AssignmentDto>? upcomingAssignments = [];
    private IEnumerable<ModuleDto>? modules = [];
    private ModuleDto? currentModule;
    private int courseId;
    public UserDto? User { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        try
        {

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var authUser = authState.User;
            if (authUser.Identity?.IsAuthenticated == true)
            {
                var userId = authUser.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    User = await ApiService.CallApiAsync<UserDto>($"api/users/{userId}");
                    if (User != null && User.CourseId.HasValue)
                        courseId = User.CourseId.Value;
                }
            }
            var requestParams = new RequestParams() { OrderBy = OrderByParams.DateAsc, PageSize = 100 };
            var queryString = QueryStringHelper.ObjectToQueryString(requestParams);
            modules = await ApiService.CallApiAsync<IEnumerable<ModuleDto>>($"api/courses/{courseId}/Module{queryString}");
            if (modules != null && modules.Any())
            {
                currentModule = modules.FirstOrDefault(m => m.EndsAt > DateTime.Now);
            }

            await GetStats();
            await GetUpcomingActivities();
            await GetUpcomingAssignments();
        }
        catch (HttpRequestException ex)
        {
            errorMessage = $"Ett fel uppstod när data skulle hämtas: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }

    }
    private async Task GetStats()
    {
        stats = await ApiService.CallApiAsync<StudentDashboardStatsDto>($"api/dashboard/student");
    }

    private async Task GetUpcomingActivities()
    {
        var requestParams = new RequestParams() { OrderBy = OrderByParams.DateAsc, PageSize = 100 };
        var queryString = QueryStringHelper.ObjectToQueryString(requestParams);
        if (currentModule != null)
            upcomingActivities = await ApiService.CallApiAsync<IEnumerable<ActivityDto>>($"api/modules/{currentModule.Id}/activities{queryString}");

    }

    private async Task GetUpcomingAssignments()
    {
        if (currentModule != null)
        {
            upcomingAssignments = await ApiService.CallApiAsync<IEnumerable<AssignmentDto>>($"api/modules/{currentModule.Id}/activities/assignments");
        }
    }
}