using FCG_MS_Users.Application.Dtos;
using FCG_MS_Users.Application.Interfaces;
using FCG_MS_Users.Domain.Entities;
using FCG_MS_Users.Domain.Enums;
using FCG_MS_Users.Domain.Exceptions;
using FCG_MS_Users.Domain.Interfaces;
using FCG_MS_Users.Domain.ValueObjects;
using FCG_MS_Users.Infrastructure.ExternalClients.Interfaces;

namespace FCG_MS_Users.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;
    private readonly IUserNotificationClient _userNotificationClient;

    public UserService(
        IUserRepository userRepository,
        IUserAuthorizationRepository userAuthorizationRepository,
        IUserNotificationClient userNotificationClient)
    {
        _userRepository = userRepository;
        _userAuthorizationRepository = userAuthorizationRepository;
        _userNotificationClient = userNotificationClient;
    }

    public async Task<ResponseUserDto> RegisterUserAsync(RegisterUserDto userDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUser != null)
        {
            throw new DomainException("Email already exists");
        }
        var emailVo = new Email(userDto.Email);
        var passwordVo = new Password(userDto.Password);

        var user = new User(userDto.Name, emailVo, passwordVo);
        await _userRepository.AddAsync(user);

        var userAuthorization = new UserAuthorization(user.Id, AuthorizationPermissions.Admin);
        await _userAuthorizationRepository.AddAsync(userAuthorization);

        var userReponseDto = new ResponseUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Permission = AuthorizationPermissions.Admin.ToString(),
        };

        //await _userNotificationClient.SendemailAsync(userReponseDto.Email);

        return userReponseDto;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var emailVo = new Email(email);
        return await _userRepository.GetByEmailAsync(emailVo.Value);
    }

    public async Task<List<ResponseUserDto>> SearchUsersAsync(string email, string name)
    {
        var users = await _userRepository.SearchUsersAsync(email, name);

        var responseUserDto = new List<ResponseUserDto>();

        foreach (var user in users)
        {
            var response = new ResponseUserDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Permission = user.Authorization.Permission.ToString(),
            };
            responseUserDto.Add(response);
        }

        return responseUserDto;
    }

    public async Task<string?> UpdateAsync(UpdateUserDto userDto)
    {
        if (userDto == null)
            return "Request invalid is not allowed to be null";

        var userResponse = await GetUserByIdAsync(userDto.UserId);

        if (userResponse == null)
            return "UserId does not exist";

        var isEmailExist = await _userRepository.SearchUsersAsync(userDto.Email, name: null);

        if (isEmailExist.Count > 0) return "Email indicated already exists in the database";

        userResponse.SetName(userDto.Name);

        if (userDto.Email != null)
            userResponse.SetEmail(userDto.Email);

        await _userRepository.UpdateAsync(userResponse);

        var updatedUser = await GetUserByIdAsync(userResponse.Id);

        return "Usuario Atualizado com sucesso";
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);

        if (user == null)
            return;

        await _userRepository.DeleteAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}
