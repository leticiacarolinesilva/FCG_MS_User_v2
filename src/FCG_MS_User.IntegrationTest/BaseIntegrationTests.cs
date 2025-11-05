using FCG_MS_Users.Infra;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Testcontainers.PostgreSql;
using Xunit;

namespace FCG_MS_Users.IntegrationTest;

public class BaseIntegrationTests : IAsyncLifetime
{
    protected HttpClient HttpClient { get; private set; }
    protected readonly PostgreSqlContainer DbContainer;
    protected UserRegistrationDbContext DbContext;
    private WebApplicationFactory<Program> _factory;

    public BaseIntegrationTests()
    {
        DbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSolutionRelativeContentRoot(
                    Path.Combine("src", "FCG_MS_Users.Api"));

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<UserRegistrationDbContext>>();
                    services.AddDbContext<UserRegistrationDbContext>(options =>
                        options.UseNpgsql(DbContainer.GetConnectionString()));
                });
            });

        HttpClient = _factory.CreateClient();

        HttpClient.DefaultRequestHeaders.Authorization = GetToken();

        // Create a scope to resolve DbContext
        var scope = _factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<UserRegistrationDbContext>();
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }
        await DbContainer.DisposeAsync();
        HttpClient?.Dispose();
        _factory?.Dispose();
    }

    public AuthenticationHeaderValue GetToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtKey = Encoding.ASCII.GetBytes("S2V5Snd0VXNlclJlZ2lzdHJhdGlvbkFuZEdhbWVMaWJyYXJ5");

        var tokenPropriedades = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name,"Test User"),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenPropriedades);

        return new AuthenticationHeaderValue("Bearer", tokenHandler.WriteToken(token));
    }
}
