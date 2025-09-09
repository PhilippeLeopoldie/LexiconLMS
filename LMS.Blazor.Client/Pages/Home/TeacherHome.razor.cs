using LMS.Shared.Common;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.DTOs.DashboardDtos;
using LMS.Shared.DTOs.DocumentDtos;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Enums;
using LMS.Shared.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LMS.Blazor.Client.Pages.Home;
public partial class TeacherHome
{
    private bool isLoading = true;
    private string? errorMessage;
    private TeacherDashboardStatsDto? stats = new();
    private IEnumerable<CourseDto>? courses = [];
    private IEnumerable<DocumentDto>? documents = [];
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
                }
            }
            await GetStats();
            await GetCourses();
            await GetRecentDocuments();
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
        stats = await ApiService.CallApiAsync<TeacherDashboardStatsDto>("api/dashboard/teacher");
    }
    private async Task GetCourses()
    {
        var requestParams = new RequestParams() { OrderBy = OrderByParams.DateDesc, PageSize = 6 };
        var queryString = QueryStringHelper.ObjectToQueryString(requestParams);

        courses = await ApiService.CallApiAsync<IEnumerable<CourseDto>>($"api/courses/{queryString}&includeModules=true&includeUsers={UserRole.Student}");
    }

    private async Task GetRecentDocuments()
    {

        var requestParams = new RequestParams() { OrderBy = OrderByParams.DateDesc, PageSize = 6 };
        var queryString = QueryStringHelper.ObjectToQueryString(requestParams);
        documents = await ApiService.CallApiAsync<IEnumerable<DocumentDto>>($"api/documents/{queryString}");
    }
}