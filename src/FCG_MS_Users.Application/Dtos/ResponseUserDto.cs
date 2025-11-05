namespace FCG_MS_Users.Application.Dtos;

public class ResponseUserDto
{
    /// <summary>
    /// User Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User Permission
    /// </summary>
    public string Permission { get; set; } = string.Empty;
}
