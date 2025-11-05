using FCG_MS_Users.Domain.Entities;

namespace FCG_MS_Users.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<List<User>> SearchUsersAsync(string? email, string? name);
}
