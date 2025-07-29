namespace Base.Interfaces;

public interface INotificationService
{
    Task SendSms(string phoneNumber, string message);
    Task SendEmail(string email, string subject, string message);
}
