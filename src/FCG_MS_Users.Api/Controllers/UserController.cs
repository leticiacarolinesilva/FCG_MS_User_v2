using FCG_MS_Users.Api.Filters;
using FCG_MS_Users.Application.Dtos;
using FCG_MS_Users.Application.Interfaces;
using FCG_MS_Users.Domain.Enums;
using FCG_MS_Users.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCG_MS_Users.Api.Controllers;

/// <summary>
/// API for user management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _uservice;

    public UserController(IUserService uservice)
    {
        _uservice = uservice;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">ser registration data</param>
    /// <returns>The newly created user</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ResponseUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(request?.Name))
            {
                var user = await _uservice.RegisterUserAsync(request);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            return BadRequest();
        }
        catch (DomainException ex)
        {

            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get user by email, requires an Admin token
    /// </summary>
    /// <param name="email">Email user</param>
    /// <param name="name">Name user</param>
    /// <returns>User properties</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ResponseUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [UserAuthorizeAtribute(AuthorizationPermissions.Admin, AuthorizationPermissions.User)]
    public async Task<IActionResult> GetUser([FromQuery] string? email, string? name)
    {
        var response = await _uservice.SearchUsersAsync(email, name);

        if (!response.Any())
        {
            return NotFound();
        }

        return Ok(response);
    }

    /// <summary>
    /// Get user by Id, requires an Admin or User token
    /// </summary>
    /// <param name="Id">Id user</param>
    /// <returns>User properties</returns>
    [HttpGet("id")]
    [ProducesResponseType(typeof(ResponseUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [UserAuthorizeAtribute(AuthorizationPermissions.User, AuthorizationPermissions.Admin)]
    public async Task<IActionResult> GetUserById([FromQuery] Guid Id)
    {
        var response = await _uservice.GetUserByIdAsync(Id);

        if (response == null)
        {
            return NotFound();
        }

        var responseDto = new ResponseUserDto()
        {
            Id = response.Id,
            Name = response.Name,
            Email = response.Email,
            Permission = response.Authorization.Permission.ToString(),
        };

        return Ok(responseDto);
    }

    /// <summary>
    /// Update user , requires an Admin or User token
    /// </summary>
    /// <param name="userDto">Dto for update user</param>
    /// <returns>User properties</returns>
    [HttpPut]
    [ProducesResponseType(typeof(List<ResponseUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [UserAuthorizeAtribute(AuthorizationPermissions.Admin, AuthorizationPermissions.User)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto userDto)
    {
        var response = await _uservice.UpdateAsync(userDto);

        return Ok(response);
    }

    /// <summary>
    /// Delete user by Id, requires an Admin token
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>User properties</returns>
    [HttpDelete]
    [ProducesResponseType(typeof(List<ResponseUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [UserAuthorizeAtribute(AuthorizationPermissions.Admin)]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid userId)
    {
        await _uservice.DeleteAsync(userId);

        return NoContent();
    }
}
