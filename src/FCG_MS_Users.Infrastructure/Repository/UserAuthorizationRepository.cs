using FCG_MS_Users.Domain.Entities;
using FCG_MS_Users.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG_MS_Users.Infra.Repository;

public class UserAuthorizationRepository : IUserAuthorizationRepository
{
    private readonly UserRegistrationDbContext _context;

    public UserAuthorizationRepository(UserRegistrationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserAuthorization userAuthorization)
    {
        await _context.UserAuthorizations.AddAsync(userAuthorization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserAuthorization userAuthorization)
    {
        _context.UserAuthorizations.Update(userAuthorization);
        await _context.SaveChangesAsync();
    }

    public async Task<UserAuthorization?> GetByIdAsync(Guid id)
    {
        return await _context.UserAuthorizations
            .FirstOrDefaultAsync(gl => gl.UserId == id);
    }
}
