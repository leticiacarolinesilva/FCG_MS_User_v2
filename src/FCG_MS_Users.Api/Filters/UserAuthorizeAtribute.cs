using FCG_MS_Users.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FCG_MS_Users.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class UserAuthorizeAtribute : TypeFilterAttribute
{
    public UserAuthorizeAtribute(params AuthorizationPermissions[] mandatory)
        : base(typeof(AuthorizationUserFilter))
    {
        base.Arguments = new object[] { mandatory };
    }
}
