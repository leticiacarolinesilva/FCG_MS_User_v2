using FCG_MS_Users.Api.Filters;
using FCG_MS_Users.Application.Dtos;
using FCG_MS_Users.Application.Interfaces;
using FCG_MS_Users.Domain.Enums;
using FCG_MS_Users.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCG_MS_Users.Api.Controllers;

/// <summary>
/// API for authentication management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserAuthorizationController : ControllerBase
{
    private readonly IUserAuthorizationService _authService;

    public UserAuthorizationController(IUserAuthorizationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Get token for user
    /// </summary>
    /// <param name="request">user data</param>
    /// <returns>Token JWT</returns>
    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] AuthorizationTokenDto request)
    {
        try
        {
            if (!string.IsNullOrEmpty(request?.Email!))
            {
                var response = await _authService.GetToken(request);

                return Ok(response);
            }

            return BadRequest();
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Assign authorization to user, requires an Admin token
    /// </summary>
    /// <param name="request">Autorization data</param>
    /// <returns>Status code created</returns>
    [HttpPost("user-permissions")]
    [UserAuthorizeAtribute(AuthorizationPermissions.Admin)]
    public async Task<IActionResult> AddPermissionByUser([FromBody] UserAuthorizationDto request)
    {
        try
        {
            var response = await _authService.AddPermissionByUserAsync(request);

            return Ok(response);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// update authorization to user, requires an Admin token
    /// </summary>
    /// <param name="request">Autorization data</param>
    /// <returns>Status code created</returns>
    [HttpPut("user-permissions")]
    [UserAuthorizeAtribute(AuthorizationPermissions.Admin)]
    public async Task<IActionResult> UpdatePermissionByUser([FromBody] UserAuthorizationDto request)
    {
        try
        {
            var response = await _authService.UpdatePermissionByUserAsync(request);

            return Ok(response);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
