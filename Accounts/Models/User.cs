using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using Base.Interfaces;
using Base.Utilities;
using Accounts.Data;

namespace Accounts.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ProfilePicture { get; set; }
    public DateOnly RegisteredAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public float WalletBalance { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}";

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

    public async Task SaveProfilePicture(IFormFile profilePicture, string rootPath)
    {
        string dir = "media/profile_pictures";
        if (!Directory.Exists(Path.Combine(rootPath, dir)))
            Directory.CreateDirectory(Path.Combine(rootPath, dir));
        await profilePicture.SaveAsWebP(Path.Combine(rootPath, dir, Id.ToString()));
        ProfilePicture = Path.Combine(dir, $"{Id}.webp");
    }
}
