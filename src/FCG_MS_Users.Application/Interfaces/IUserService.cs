using FCG_MS_Users.Application.Dtos;
using FCG_MS_Users.Domain.Entities;

namespace FCG_MS_Users.Application.Interfaces;

public interface IUserService
{
    Task<ResponseUserDto> RegisterUserAsync(RegisterUserDto dto);
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<ResponseUserDto>> SearchUsersAsync(string email, string name);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<string> UpdateAsync(UpdateUserDto user);
    Task DeleteAsync(Guid userId);
}
