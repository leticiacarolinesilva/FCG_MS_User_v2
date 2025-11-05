namespace FCG_MS_Users.Infrastructure.ExternalClients.Interfaces;

public interface IUserNotificationClient
{
    Task SendemailAsync(string email);
}
