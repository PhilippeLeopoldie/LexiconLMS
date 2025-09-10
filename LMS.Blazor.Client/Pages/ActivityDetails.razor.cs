using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.DocumentDtos;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;

namespace LMS.Blazor.Client.Pages;
public partial class ActivityDetails
{
    [Parameter] public int ActivityId { get; set; }
    [Parameter] public int ModuleId { get; set; }
    [Parameter] public int CourseId { get; set; }

    private bool isLoading = true;
    private string? errorMessage;
    private ActivityDto? activity;
    private DocumentDto? document;
    private bool isDownloading = false;
    private int? downloadingDocumentId = null;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            activity = await ApiService.CallApiAsync<ActivityDto>($"api/modules/{ModuleId}/activities/{ActivityId}");
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                errorMessage = "Aktiviteten kunde inte hittas.";
            }
            else
            {
                errorMessage = $"Ett fel uppstod vid hämtning av data: {ex.Message}";
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo($"/courses/{CourseId}/modules/{ModuleId}/activities");
    }

    private string GetActivityTypeIcon(int? activityTypeId)
    {
        if (!activityTypeId.HasValue) return "braces";

        return activityTypeId.Value switch
        {
            1 => "book",
            2 => "globe",
            3 => "code-slash",
            4 => "feather",
            _ => "braces",
        };
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
}