using LMS.Blazor.Client.Services;
using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.ModuleDtos;
using LMS.Shared.Enums;
using LMS.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LMS.Blazor.Client.Pages;
public partial class ModuleActivities : ComponentBase
{

    [Inject]
    private IApiService _apiService { get; set; } = default!;

    [Parameter]
    public int moduleId { get; set; }

    [Parameter]
    public int courseId { get; set; }

    private ModuleDto? module;
    private IEnumerable<ActivityTypeDto>? activityTypes = default!;
    private IEnumerable<ActivityDto>? activities = [];
    private IEnumerable<ActivityDto> upcomingActivities = [];
    private IEnumerable<ActivityDto> pastActivities = [];
    private string? filterText;
    private int selectedActivityTypeId = 0;
    private DateTime? firstUpcomingDate;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            module = await _apiService.CallApiAsync<ModuleDto>($"api/courses/{courseId}/module/{moduleId}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Fel vid hämtning av modul: {ex.Message}");
            activities = [];
        }
        await LoadActivities();
    }

    private async Task LoadActivities()
    {
        try
        {
            activityTypes = await _apiService.CallApiAsync<IEnumerable<ActivityTypeDto>>("api/ActivityTypes");
            activityTypes = activityTypes ?? [];

            var requestParams = new RequestParams() { SearchTerm = filterText, Page = 1, OrderBy = OrderByParams.DateAsc, PageSize = 100 };
            var queryString = QueryStringHelper.ObjectToQueryString(requestParams);

            activities = await _apiService.CallApiAsync<IEnumerable<ActivityDto>>($"api/modules/{moduleId}/activities?{queryString}");
            activities = activities ?? [];

            if (selectedActivityTypeId != 0)
                activities = activities.Where(m => m.ActivityType.Id == selectedActivityTypeId);

            if (activities.Any())
            {
                upcomingActivities = [.. activities.Where(a => a.StartsAt.Date >= DateTime.Today)];
                pastActivities = [.. activities.Where(a => a.StartsAt.Date < DateTime.Today)];

                firstUpcomingDate = upcomingActivities.FirstOrDefault()?.StartsAt.Date;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ett fel uppstod: {ex.Message}");
            activities = [];

            upcomingActivities = [];
            pastActivities = [];
        }
    }

    private bool IsFirstUpcoming(DateTime date) => date.Date == firstUpcomingDate?.Date;

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await LoadActivities();
    }

    private async Task OnActivityTypeChanged(ChangeEventArgs args)
    {
        selectedActivityTypeId = int.Parse(args.Value?.ToString() ?? "0");
        await LoadActivities();
    }

    private async Task DeleteActivity(int activityId)
    {
        try
        {
            await _apiService.DeleteAsync($"api/modules/{moduleId}/activities/{activityId}");
            await LoadActivities();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Fel vid borttagning: {ex.Message}");
        }
    }


    private string GetCssColorForActivityType(int? activityTypeId)
    {
        if (!activityTypeId.HasValue) return "default";

        return activityTypeId.Value switch
        {
            1 => "warning",
            2 => "danger",
            3 => "success",
            4 => "primary",
            _ => "default",
        };
    }
}