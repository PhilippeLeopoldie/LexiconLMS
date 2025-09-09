using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace LMS.Services;
public class FakeEmailSender : IEmailSender<ApplicationUser>
{

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        Debug.WriteLine($"To: {email}, Subject: Confirm your email, Link: {confirmationLink}");
        Console.WriteLine($"To: {email}, Subject: Confirm your email, Link: {confirmationLink}");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        Debug.WriteLine($"To: {email}, Subject: Reset your password, Link: {resetLink}");
        Console.WriteLine($"To: {email}, Subject: Reset your password, Link: {resetLink}");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        Debug.WriteLine($"To: {email}, Subject: Reset your password, Code: {resetCode}");
        Console.WriteLine($"To: {email}, Subject: Reset your password, Code: {resetCode}");
        return Task.CompletedTask;
    }
}
