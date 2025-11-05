using FCG_MS_Users.Application.Dtos;
using FCG_MS_Users.Application.Interfaces;
using FCG_MS_Users.Domain.Entities;
using FCG_MS_Users.Domain.Exceptions;
using FCG_MS_Users.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCG_MS_Users.Application.Services;

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;

    public UserAuthorizationService(
        IUserService userService,
        IConfiguration configuration,
        IUserAuthorizationRepository userAuthorizationRepository)
    {
        _userService = userService;
        _configuration = configuration;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    public async Task<string> GetToken(AuthorizationTokenDto request)
    {
        try
        {
            var responseUser = await _userService.GetUserByEmailAsync(request?.Email!);

            if (responseUser is null)
                return "User does not exist";

            var isPasswordValid = responseUser.Password.Verify(request.Password);
            if (!isPasswordValid)
            {
                return "Invalid credentials";
            }

            var responseUserAuth = await _userAuthorizationRepository.GetByIdAsync(responseUser.Id);

            if (responseUserAuth is null)
                return "User does not have permission";

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var tokenPropriedades = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, responseUser.Name),
                new Claim(ClaimTypes.Role, responseUserAuth.Permission.ToString())
            }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenPropriedades);

            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new DomainException("An error occurred while obtaining the token", ex);
        }
    }

    public async Task<string> AddPermissionByUserAsync(UserAuthorizationDto request)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(request.UserId);

            if (user is null) return "User does not exist";

            var responseUserAuth = await _userAuthorizationRepository.GetByIdAsync(user.Id);

            if (responseUserAuth is not null)
                return "User does have permission";

            var userAuthorization = new UserAuthorization(request.UserId, request.Permission);

            await _userAuthorizationRepository.AddAsync(userAuthorization);

            return $"Permission successfully registered for user: Email: {user.Email}, Permission: {userAuthorization.Permission}";
        }
        catch (Exception ex)
        {
            throw new DomainException("An error occurred while user permission registered", ex);
        }
    }

    public async Task<string> UpdatePermissionByUserAsync(UserAuthorizationDto request)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(request.UserId);

            if (user is null) return "User does not exist";

            var responseUserAuth = await _userAuthorizationRepository.GetByIdAsync(user.Id);

            if (responseUserAuth is null)
                return "User does not have permission";

            var userAuthorization = new UserAuthorization(request.UserId, request.Permission);

            responseUserAuth.ChangePermission(request.Permission);

            await _userAuthorizationRepository.UpdateAsync(responseUserAuth);

            return $"Permission successfully updated for user: Email: {user.Email}, Permission: {userAuthorization.Permission}";
        }
        catch (Exception ex)
        {
            throw new DomainException("An error occurred while user permission updated", ex);
        }
    }
}
