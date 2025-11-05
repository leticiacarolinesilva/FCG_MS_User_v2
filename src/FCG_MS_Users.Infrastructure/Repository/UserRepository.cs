using FCG_MS_Users.Domain.Entities;
using FCG_MS_Users.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG_MS_Users.Infra.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserRegistrationDbContext _context;

    public UserRepository(UserRegistrationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Authorization)
            .FirstOrDefaultAsync(gl => gl.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(gl => gl.Email.Value == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> SearchUsersAsync(string? email, string? name)
    {
        var query = _context.Users
            .Include(u => u.Authorization)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(u => u.Email.Value == email);
        }

        return await query.ToListAsync();
    }
}
