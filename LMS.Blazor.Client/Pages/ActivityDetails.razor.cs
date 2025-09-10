using LMS.Shared.DTOs.ActivityDtos;
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
    private bool showUploadForm = false;
    private bool isTeacher = false;
    private int? downloadingDocumentId = null;
    private string errorMessage = string.Empty;
    private IBrowserFile? selectedFile = null;
    private string selectedFileName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CheckUserRole();
        await LoadActivity();
    }

    private async Task CheckUserRole()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            isTeacher = authState.User.IsInRole("Teacher");
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

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
        selectedFileName = e.File.Name;
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

    private void CancelUpload()
    {
        selectedFile = null;
        selectedFileName = string.Empty;
        showUploadForm = false;
        errorMessage = string.Empty;
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

    private void NavigateBack()
    {
        NavigationManager.NavigateTo($"/courses/{CourseId}/modules/{ModuleId}/activities");
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