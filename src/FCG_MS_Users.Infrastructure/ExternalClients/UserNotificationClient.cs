using FCG_MS_Users.Domain.Exceptions;
using FCG_MS_Users.Infrastructure.ExternalClients.Interfaces;

namespace FCG_MS_Users.Infrastructure.ExternalClients;

public class UserNotificationClient : IUserNotificationClient
{
    private readonly HttpClient _httpClient;

    public UserNotificationClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendemailAsync(string email)
    {
        var response = await _httpClient.PostAsync($"/api/v1/user/notification-email?email={email}", default);

        if (!response.IsSuccessStatusCode)
        {
            throw new DomainException($"Failed to deserialize response from user service. {response}, Url{response.Content}");
        }
    }
}
