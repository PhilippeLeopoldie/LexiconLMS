using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Enums;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace LMS.Blazor.Client.Pages;
public partial class UserForm
{
    [Parameter]
    public string? userId { get; set; }

    private bool isLoading = true;
    private string? errorMessage;
    private UserFormModel user = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        if (!authState.User.IsInRole("Teacher"))
        {
            _navigationManager.NavigateTo("/Account/AccessDenied");
            return;
        }

        try
        {
            var existing = await _apiService.CallApiAsync<UserBasicDto>($"api/users/{userId}");
            if (existing != null)
            {
                user = new UserFormModel
                {
                    UserName = existing.UserName,
                    FullName = $"{existing.FirstName} {existing.LastName}",
                    Email = existing.Email,
                    FirstName = existing.FirstName,
                    LastName = existing.LastName,
                    PhoneNumber = existing.PhoneNumber,
                    Role = existing!.Role,
                    CourseId = existing.CourseId
                };
            }
        }
        catch (HttpRequestException ex)
        {
            errorMessage = ex.Message;
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
            var edit = new UserUpdateDto(user.UserName, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.Role, user.CourseId);
            await _apiService.PutAsync($"api/users/{userId}", edit);
            NavigateBack();
        }
        catch (HttpRequestException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }

    private void NavigateBack()
    {
        _navigationManager.NavigateTo("/Account/UserManagement");
    }

    private sealed class UserFormModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;
        public int? CourseId { get; set; }
    }
}
