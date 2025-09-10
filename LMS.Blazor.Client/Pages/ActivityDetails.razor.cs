using LMS.Shared.Common;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.DocumentDtos;
using LMS.Shared.Enums;
using LMS.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace LMS.Blazor.Client.Pages;

public partial class ActivityDetails
{
    [Parameter] public int CourseId { get; set; }
    [Parameter] public int ModuleId { get; set; }
    [Parameter] public int ActivityId { get; set; }

    private ActivityDto? activity;
    private bool isLoading = true;
    private bool isDownloading = false;
    private bool isUploading = false;
    private bool isSubmitting = false;
    private bool showUploadForm = false;
    private bool showSubmissionForm = false;
    private bool isTeacher = false;
    private int? downloadingDocumentId = null;
    private string errorMessage = string.Empty;
    private IBrowserFile? selectedFile = null;
    private string selectedFileName = string.Empty;
    private IBrowserFile? selectedSubmissionFile = null;
    private string selectedSubmissionFileName = string.Empty;
    private List<DocumentDto> submissions = new();
    private List<DocumentDto> userSubmissions = new();
    private string? currentUserId;

    protected override async Task OnInitializedAsync()
    {
        await CheckUserRole();
        await LoadActivity();
        if (activity?.ActivityType.Id == 4) // Inlämningsuppgift
            await LoadSubmissions();
    }

    private async Task CheckUserRole()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            isTeacher = authState.User.IsInRole("Teacher");
            currentUserId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }

    private async Task LoadActivity()
    {
        try
        {
            isLoading = true;
            errorMessage = string.Empty;
            activity = await ApiService.CallApiAsync<ActivityDto>($"api/modules/{ModuleId}/activities/{ActivityId}");
        }
        catch (Exception ex)
        {
            errorMessage = $"Ett fel uppstod när aktiviteten laddades: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task LoadSubmissions()
    {
        var requestParams = new RequestParams() { OrderBy = OrderByParams.DateAsc, PageSize = 100 };
        var queryString = QueryStringHelper.ObjectToQueryString(requestParams);

        try
        {
            if (isTeacher)
            {
                var allSubmissions = await ApiService.CallApiAsync<IEnumerable<DocumentDto>>($"api/documents/submissions/{ActivityId}/{queryString}");
                submissions = allSubmissions.ToList();
            }
            else if (currentUserId != null)
            {
                var mySubmissions = await ApiService.CallApiAsync<IEnumerable<DocumentDto>>($"api/documents/user/{currentUserId}/{queryString}");
                userSubmissions = mySubmissions.ToList();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Ett fel uppstod när inlämningar laddades: {ex.Message}";
        }
    }

    private async Task DownloadDocument(int documentId, string fileName)
    {
        try
        {
            isDownloading = true;
            downloadingDocumentId = documentId;
            StateHasChanged();

            var fileBytes = await ApiService.DownloadFileAsync($"api/documents/{documentId}/download");

            var base64 = Convert.ToBase64String(fileBytes);
            var mimeType = GetMimeType(fileName);

            await JSRuntime.InvokeVoidAsync("downloadFile", base64, fileName, mimeType);
        }
        catch (Exception ex)
        {
            errorMessage = $"Ett fel uppstod när filen laddades ner: {ex.Message}";
        }
        finally
        {
            isDownloading = false;
            downloadingDocumentId = null;
            StateHasChanged();
        }
    }

    private void ToggleUploadForm()
    {
        showUploadForm = !showUploadForm;
        if (!showUploadForm)
        {
            CancelUpload();
        }
    }

    private void ToggleSubmissionForm()
    {
        showSubmissionForm = !showSubmissionForm;
        if (!showSubmissionForm)
        {
            CancelSubmission();
        }
    }

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
        selectedFileName = e.File.Name;
        errorMessage = string.Empty;
    }

    private void OnSubmissionFileSelected(InputFileChangeEventArgs e)
    {
        selectedSubmissionFile = e.File;
        selectedSubmissionFileName = e.File.Name;
        errorMessage = string.Empty;
    }

    private async Task UploadDocument()
    {
        if (selectedFile == null) return;

        try
        {
            isUploading = true;
            StateHasChanged();

            // Validate file size (10MB limit)
            const long maxFileSize = 10 * 1024 * 1024;
            if (selectedFile.Size > maxFileSize)
            {
                errorMessage = "Filen är för stor. Maximal storlek är 10MB.";
                return;
            }

            var formData = new Dictionary<string, string>
            {
                ["activityId"] = ActivityId.ToString()
            };

            using var stream = selectedFile.OpenReadStream(maxFileSize);
            var contentType = selectedFile.ContentType ?? GetMimeType(selectedFile.Name);

            var result = await ApiService.UploadFileAsync<int>($"api/documents/upload?activityId={activity.Id}", stream, selectedFile.Name, contentType, formData);

            await LoadActivity();

            CancelUpload();

            errorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Ett fel uppstod när filen laddades upp: {ex.Message}";
        }
        finally
        {
            isUploading = false;
            StateHasChanged();
        }
    }

    private async Task SubmitAssignment()
    {
        if (selectedSubmissionFile == null) return;

        try
        {
            isSubmitting = true;
            StateHasChanged();

            // Validate file size (10MB limit)
            const long maxFileSize = 10 * 1024 * 1024;
            if (selectedSubmissionFile.Size > maxFileSize)
            {
                errorMessage = "Filen är för stor. Maximal storlek är 10MB.";
                return;
            }

            var formData = new Dictionary<string, string>
            {
                ["activityId"] = ActivityId.ToString()
            };

            using var stream = selectedSubmissionFile.OpenReadStream(maxFileSize);
            var contentType = selectedSubmissionFile.ContentType ?? GetMimeType(selectedSubmissionFile.Name);

            var result = await ApiService.UploadFileAsync<int>($"api/documents/upload?activityId={activity.Id}", stream, selectedSubmissionFile.Name, contentType, formData);

            await LoadSubmissions();

            CancelSubmission();

            errorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            errorMessage = $"Ett fel uppstod när uppgiften lämnades in: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    private void CancelUpload()
    {
        selectedFile = null;
        selectedFileName = string.Empty;
        showUploadForm = false;
        errorMessage = string.Empty;
    }

    private void CancelSubmission()
    {
        selectedSubmissionFile = null;
        selectedSubmissionFileName = string.Empty;
        showSubmissionForm = false;
        errorMessage = string.Empty;
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo($"/courses/{CourseId}/modules/{ModuleId}/activities");
    }

    private static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };
    }

    private static string GetActivityTypeIcon(int activityTypeId)
    {
        return activityTypeId switch
        {
            1 => "book", // Föreläsning
            2 => "laptop", // E-learning
            3 => "code-slash",
            4 => "pencil-square", // Uppgift
            _ => "calendar-event"
        };
    }
}