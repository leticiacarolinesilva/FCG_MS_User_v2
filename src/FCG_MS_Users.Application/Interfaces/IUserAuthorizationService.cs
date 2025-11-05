using FCG_MS_Users.Application.Dtos;

namespace FCG_MS_Users.Application.Interfaces;
public interface IUserAuthorizationService
{
    public Task<string> GetToken(AuthorizationTokenDto request);
    Task<string> AddPermissionByUserAsync(UserAuthorizationDto request);
    Task<string> UpdatePermissionByUserAsync(UserAuthorizationDto request);
}
