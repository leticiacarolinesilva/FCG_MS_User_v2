namespace FCG_MS_Users.Application.Dtos;

public class UpdateUserDto
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User's full name
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// User's email address will be used for authentication
    /// </summary>
    public string Email { get; set; }
}
