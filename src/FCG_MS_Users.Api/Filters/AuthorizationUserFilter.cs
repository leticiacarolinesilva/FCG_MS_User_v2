using FCG_MS_Users.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FCG_MS_Users.Api.Filters;

public class AuthorizationUserFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    protected readonly AuthorizationPermissions[] _mandatoryPermissions;

    public AuthorizationUserFilter(
        IConfiguration configuration,
        AuthorizationPermissions[] mandatoryPermissions)
    {
        _configuration = configuration;
        _mandatoryPermissions = mandatoryPermissions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorizationHeader = context?.HttpContext.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Jwt:Key"];
            var key = Encoding.UTF8.GetBytes(secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(30)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (string.IsNullOrEmpty(role))
            {
                context.Result = new ForbidResult();
                return;
            }

            var mandatoryRoles = _mandatoryPermissions.Select(x => x.ToString());
            if (!mandatoryRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
