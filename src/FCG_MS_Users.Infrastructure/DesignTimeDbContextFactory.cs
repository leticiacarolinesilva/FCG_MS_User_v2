using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FCG_MS_Users.Infra;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserRegistrationDbContext>
{
    public UserRegistrationDbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<UserRegistrationDbContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            o => o.MigrationsAssembly("FCG_MS_Users.Infrastructure"));

        return new UserRegistrationDbContext(optionsBuilder.Options);
    }
}
