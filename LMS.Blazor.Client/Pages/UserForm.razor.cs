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
                var existing = await _apiService.CallApiAsync<UserBasicDto>($"api/users/{userId}");
                if (existing != null)
                {
                    user = new UserFormModel
                    {
                        UserName = existing.UserName,
                        Email = existing.Email,
                        FirstName = existing.FirstName,
                        LastName = existing.LastName,
                        PhoneNumber = existing.PhoneNumber,
                        Role = existing.Role,
                        CourseId = existing.CourseId
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
                var role = IsTeacher ? user.Role : UserRole.Student;
                var courseId = IsTeacher ? user.CourseId : null;
                var edit = new UserUpdateDto(user.UserName, user.Email, null, user.FirstName, user.LastName, user.PhoneNumber, role, courseId);

                await _apiService.PutAsync($"api/users/{userId}", edit);
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
        public string UserName { get; set; } = string.Empty;

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


