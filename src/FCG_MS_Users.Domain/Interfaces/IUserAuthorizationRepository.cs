using FCG_MS_Users.Domain.Entities;

namespace FCG_MS_Users.Domain.Interfaces;

public interface IUserAuthorizationRepository
{
    Task AddAsync(UserAuthorization userAuthorization);
    Task UpdateAsync(UserAuthorization userAuthorization);
    Task<UserAuthorization?> GetByIdAsync(Guid id);
}
