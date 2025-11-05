using FCG_MS_Users.Api.Extensions;
using FCG_MS_Users.Infra;
using FCG_MS_Users.Infrastructure.ExternalClients;
using FCG_MS_Users.Infrastructure.ExternalClients.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");

builder.Services.AddDbContext<UserRegistrationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.UseCollectionExtensions();

builder.Services.UseAuthenticationExtensions(jwtKey);

builder.Services.UseSwaggerExtensions();

builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.UseValidatorExtensions();

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "PostgreSQL");

var uriNotification = builder.Configuration["UserNotification:Uri"];

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

if (string.IsNullOrEmpty(uriNotification))
{
    throw new ArgumentException("User Client URI is not configured.", nameof(uriNotification));
}

builder.Services.AddHttpClient<IUserNotificationClient, UserNotificationClient>(client =>
{
    client.BaseAddress = new Uri(uriNotification);
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserRegistrationDbContext>();
    dbContext.Database.Migrate();
}


app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
