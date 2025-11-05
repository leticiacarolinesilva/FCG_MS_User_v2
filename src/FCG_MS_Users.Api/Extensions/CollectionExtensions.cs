using FCG_MS_Users.Application.Interfaces;
using FCG_MS_Users.Application.Services;
using FCG_MS_Users.Domain.Interfaces;
using FCG_MS_Users.Infra.Repository;

namespace FCG_MS_Users.Api.Extensions
{
    public static class CollectionExtensions
    {
        public static IServiceCollection UseCollectionExtensions(this IServiceCollection services)
        {
            services.AddScoped<IUserAuthorizationRepository, UserAuthorizationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
