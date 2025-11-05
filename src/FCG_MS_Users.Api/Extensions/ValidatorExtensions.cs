using FCG_MS_Users.Api.Controllers.FluentValidators;
using FCG_MS_Users.Application.Dtos;
using FluentValidation;

namespace FCG_MS_Users.Api.Extensions
{
    public static class ValidatorExtensions
    {
        public static IServiceCollection UseValidatorExtensions(this IServiceCollection service)
        {
            service.AddTransient<IValidator<RegisterUserDto>, RegisterUserValidator>();

            return service;
        }
    }
}
