using FCG_MS_Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG_MS_Users.Infra;

public class UserRegistrationDbContext : DbContext
{
    public UserRegistrationDbContext(DbContextOptions<UserRegistrationDbContext> options) : base(options)
    {

    }

    /// <summary>
    /// Used for EF Core
    /// </summary>
    protected UserRegistrationDbContext()
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserAuthorization> UserAuthorizations => Set<UserAuthorization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default schema for all entities
        modelBuilder.HasDefaultSchema("fcg_user");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserRegistrationDbContext).Assembly);

        modelBuilder.HasPostgresExtension("uuid-ossp");

        base.OnModelCreating(modelBuilder);
    }
}
