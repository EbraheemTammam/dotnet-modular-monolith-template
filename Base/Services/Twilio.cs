using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using SendGrid;
using SendGrid.Helpers.Mail;

using Base.Interfaces;

namespace Base.Services;

internal class TwilioService : INotificationService
{
    private readonly PhoneNumber _phoneNumber;
    private readonly EmailAddress? _fromEmail;
    private readonly string? _apiKey;

    public TwilioService()
    {
        _phoneNumber = new PhoneNumber(Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER"));
        _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        _fromEmail = new EmailAddress(Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL"), Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME"));
    }

    public async Task SendSms(string phoneNumber, string message) =>
        await MessageResource.CreateAsync(
            from: _phoneNumber,
            to: new PhoneNumber(phoneNumber),
            body: message
        );

    public async Task SendEmail(string email, string subject, string message)
    {
        var client = new SendGridClient(_apiKey);
        var msg = MailHelper.CreateSingleEmail(_fromEmail, new EmailAddress(email), subject, message, default);
        await client.SendEmailAsync(msg).ConfigureAwait(false);
    }
}
