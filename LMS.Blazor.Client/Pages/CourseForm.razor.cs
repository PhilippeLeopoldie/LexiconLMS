using LMS.Blazor.Client.Models;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class CourseForm
{
    [Parameter]
    public int? courseId { get; set; }

    private CourseDto? course;
    private CourseFormModel courseDto = new() { Name = string.Empty, StartsAtText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"), EndsAtText = DateTime.Now.AddMonths(1).ToString("yyyy-MM-ddTHH:mm") };

    private bool isLoading = true;
    private string? errorMessage;

    private bool IsEditMode => courseId.HasValue;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (IsEditMode)
            {
                var courseToEdit = await _apiService.CallApiAsync<CourseDto>($"api/courses/{courseId}");
                if (courseToEdit != null)
                {
                    courseDto = new()
                    {
                        Name = courseToEdit.Name,
                        Description = courseToEdit.Description,
                        StartsAtText = courseToEdit.Starts.ToString("yyyy-MM-ddTHH:mm"),
                        EndsAtText = courseToEdit.Ends.ToString("yyyy-MM-ddTHH:mm")
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
            if (string.IsNullOrWhiteSpace(courseDto.Name))
            {
                errorMessage = "Namn är obligatoriskt.";
                isLoading = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(courseDto.StartsAtText) || string.IsNullOrWhiteSpace(courseDto.EndsAtText))
            {
                errorMessage = "Både start- och sluttid måste anges.";
                isLoading = false;
                return;
            }

            if (!DateTime.TryParse(courseDto.StartsAtText, out var startsAt)
                || !DateTime.TryParse(courseDto.EndsAtText, out var endsAt))
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

            if (IsEditMode)
            {
                var editDto = new CourseEditDto
                {
                    Id = courseId!.Value,
                    Name = courseDto.Name,
                    Description = courseDto.Description,
                    Starts = startsAt,
                    Ends = endsAt
                };
                await _apiService.PutAsync($"api/courses/{courseId}", editDto);
            }
            else
            {
                var createDto = new CourseForCreationDto
                {
                    Name = courseDto.Name,
                    Description = courseDto.Description,
                    Starts = startsAt,
                    Ends = endsAt
                };
                await _apiService.PostAsync($"api/courses", createDto);
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
        _navigationManager.NavigateTo($"/courses");
    }
}