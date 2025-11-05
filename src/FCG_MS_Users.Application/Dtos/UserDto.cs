using System.ComponentModel.DataAnnotations;

namespace FCG_MS_Users.Application.Dtos;

public class UserDto
{
    /// <summary>
    /// Email address used for authentication
    /// </summary>
    [Required]
    public required string Email { get; set; }

    /// <summary>
    /// Hashed password 
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
