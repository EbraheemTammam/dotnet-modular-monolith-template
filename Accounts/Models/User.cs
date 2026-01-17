using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Base.Interfaces;
using Accounts.Data;

namespace Accounts.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly RegisteredAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string FullName => $"{FirstName} {LastName}";

    public async Task SendEmailConfirmation(UserManager<User> userManager, string scheme, INotificationService notificationService, IUrlHelper Url)
    {
        string token = await userManager.GenerateEmailConfirmationTokenAsync(this);
        string? confirmationLink = Url.Action(
            "ConfirmEmail",
            "Auth",
            new { userId = Id, token },
            scheme
        );

        string emailBody = $"Please confirm your email by clicking <a href='{HtmlEncoder.Default.Encode(confirmationLink!)}'>here</a>.";
        await notificationService.SendEmail(Email!, "Confirm Your Email", emailBody);
    }

    public async Task SendPhoneNumberConfirmation(INotificationService notificationService, VerificationManager verifications)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var verification = new Verification
        {
            PhoneNumber = PhoneNumber!,
            Token = code
        };
        await verifications.AddAsync(verification);
        await notificationService.SendSms(PhoneNumber!, $"Your verification code is {code}");
    }
}
