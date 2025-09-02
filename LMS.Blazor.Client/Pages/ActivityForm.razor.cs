using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;
public partial class ActivityForm
{
    [Parameter]
    public int courseId { get; set; }
    [Parameter]
    public int moduleId { get; set; }
    [Parameter]
    public int? activityId { get; set; }

    private ModuleDto? module;
    private ActivityFormModel activityDto = new() { Name = string.Empty, StartsAtText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"), EndsAtText = DateTime.Now.AddHours(1).ToString("yyyy-MM-ddTHH:mm"), ActivityTypeId = 1 };
    private IEnumerable<ActivityTypeDto>? activityTypes;
    private IEnumerable<ActivityDto>? existingActivities;
    private bool isLoading = true;
    private string? errorMessage;

    private bool IsEditMode => activityId.HasValue;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            module = await _apiService.CallApiAsync<ModuleDto>($"api/courses/{courseId}/module/{moduleId}");
            activityTypes = await _apiService.CallApiAsync<IEnumerable<ActivityTypeDto>>("api/activitytypes");

            try
            {
                existingActivities = await _apiService.CallApiAsync<IEnumerable<ActivityDto>>($"api/modules/{moduleId}/activities");
            }
            catch
            {
                existingActivities = null; // Best-effort only; server still validates
            }


            if (IsEditMode)
            {
                var activityToEdit = await _apiService.CallApiAsync<ActivityDto>($"api/modules/{moduleId}/activities/{activityId}");
                if (activityToEdit != null)
                {
                    activityDto = new()
                    {
                        Name = activityToEdit.Name,
                        Description = activityToEdit.Description,
                        StartsAtText = activityToEdit.StartsAt.ToString("yyyy-MM-ddTHH:mm"),
                        EndsAtText = activityToEdit.EndsAt.ToString("yyyy-MM-ddTHH:mm"),
                        ActivityTypeId = activityToEdit.ActivityType.Id
                    };
                }
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("RedirectToLogin");
            }
            else
            {
                errorMessage = $"Kunde inte ladda data: {ex.Message}";
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task HandleSubmit()
    {
        try
        {
            isLoading = true;
            if (string.IsNullOrWhiteSpace(activityDto.Name))
            {
                errorMessage = "Namn är obligatoriskt.";
                isLoading = false;
                return;
            }

            if (activityDto.ActivityTypeId <= 0)
            {
                errorMessage = "Välj en aktivitetstyp.";
                isLoading = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(activityDto.StartsAtText) || string.IsNullOrWhiteSpace(activityDto.EndsAtText))
            {
                errorMessage = "Både start- och sluttid måste anges.";
                isLoading = false;
                return;
            }

            if (!DateTime.TryParse(activityDto.StartsAtText, out var startsAt)
                || !DateTime.TryParse(activityDto.EndsAtText, out var endsAt))
            {
                errorMessage = "Ogiltigt datum/tid-format. Använd väljaren för datum och tid.";
                isLoading = false;
                return;
            }

            if (startsAt >= endsAt)
            {
                errorMessage = "Starttid måste vara före sluttid.";
                isLoading = false;
                return;
            }

            if (module != null && (startsAt < module.StartsAt || endsAt > module.EndsAt))
            {
                errorMessage = "Aktiviteten måste ligga inom modulens start- och sluttid.";
                isLoading = false;
                return;
            }

            if (existingActivities != null)
            {
                bool overlaps = existingActivities.Any(a => a.Id != (activityId ?? 0) && startsAt < a.EndsAt && endsAt > a.StartsAt);
                if (overlaps)
                {
                    errorMessage += "Aktiviteter får inte överlappa varandra.";
                    isLoading = false;
                    return;
                }
            }
            if (IsEditMode)
            {
                var editDto = new ActivityEditDto
                {
                    Id = activityId!.Value,
                    Name = activityDto.Name,
                    Description = activityDto.Description,
                    StartsAt = startsAt,
                    EndsAt = endsAt,
                    ActivityTypeId = activityDto.ActivityTypeId
                };
                await _apiService.PutAsync($"api/modules/{moduleId}/activities/{activityId}", editDto);
            }
            else
            {
                var createDto = new ActivityCreateDto
                {
                    Name = activityDto.Name,
                    Description = activityDto.Description,
                    StartsAt = startsAt,
                    EndsAt = endsAt,
                    ActivityTypeId = activityDto.ActivityTypeId
                };
                await _apiService.PostAsync($"api/modules/{moduleId}/activities", createDto);
            }
            NavigateBack();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                _navigationManager.NavigateTo("RedirectToLogin");
            else if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                errorMessage = "Du har inte behörighet att utföra denna åtgärd.";
            else
                errorMessage = $"Ett fel uppstod vid sparande: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void NavigateBack()
    {
        _navigationManager.NavigateTo($"/courses/{courseId}/modules/{moduleId}/activities");
    }
}