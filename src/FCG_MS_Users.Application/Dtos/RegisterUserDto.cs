using System.ComponentModel.DataAnnotations;

namespace FCG_MS_Users.Application.Dtos;

public class RegisterUserDto
{
    /// <summary>
    /// User's full name
    /// </summary>
    [Required]
    public required string Name { get; set; }
    /// <summary>
    /// User's email address will be used for authentication
    /// </summary>
    [Required]
    public required string Email { get; set; }
    /// <summary>
    /// Hashed password 
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    /// <summary>
    /// Compare Password 
    /// </summary>
    [Required]
    [Compare("Password")]
    public required string ConfirmationPassword { get; set; }
}
