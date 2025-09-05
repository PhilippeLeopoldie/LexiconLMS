using LMS.Shared.DTOs.UserDtos;
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

    private bool IsEditMode => !string.IsNullOrEmpty(userId);
    private bool IsTeacher { get; set; }
    private bool CanEditEmail => IsTeacher && !IsEditMode;

    protected override async Task OnInitializedAsync()
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        IsTeacher = authState.User.IsInRole("Teacher");

        try
        {
            if (IsEditMode)
            {
                var existing = await _apiService.CallApiAsync<UserDto>($"api/users/{userId}");
                if (existing != null)
                {
                    user = new UserFormModel
                    {
                        FirstName = existing.UserName ?? string.Empty,
                        LastName = string.Empty,
                        PhoneNumber = null,
                        Email = existing.Email,
                        Role = "Student"
                    };
                }
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
            if (IsEditMode)
            {
                var edit = new UserUpdateDto
                {
                    UserName = user.FirstName + (string.IsNullOrWhiteSpace(user.LastName) ? string.Empty : $" {user.LastName}"),
                    Email = CanEditEmail ? user.Email : null
                };
                await _apiService.PutAsync($"api/users/{userId}", edit);
            }
            else
            {
                var create = new UserCreationDto
                {
                    UserName = user.FirstName + (string.IsNullOrWhiteSpace(user.LastName) ? string.Empty : $" {user.LastName}"),
                    Email = user.Email!
                };
                await _apiService.PostAsync("api/users", create);
            }
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
        _navigationManager.NavigateTo("/users");
    }

    private sealed class UserFormModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Student";
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
        public int? CourseId { get; set; }
    }

}


