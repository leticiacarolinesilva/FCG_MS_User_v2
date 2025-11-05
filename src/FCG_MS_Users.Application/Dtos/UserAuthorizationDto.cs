using FCG_MS_Users.Domain.Enums;

namespace FCG_MS_Users.Application.Dtos;

public class UserAuthorizationDto
{
    public required Guid UserId { get; set; }
    public required AuthorizationPermissions Permission { get; set; }
}
