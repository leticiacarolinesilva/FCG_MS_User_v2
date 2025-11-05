using FCG_MS_Users.Domain.Enums;
using FCG_MS_Users.Domain.ValueObjects;

namespace FCG_MS_Users.Domain.Entities;

/// <summary>
/// Represents a user in the digital fcg user
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public Guid Id { get; private set; }
    /// <summary>
    /// User's full name
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// User's email address will be used for authentication
    /// </summary>
    public Email Email { get; private set; }
    /// <summary>
    /// Hashed password 
    /// </summary>
    public Password Password { get; private set; }
    /// <summary>
    /// Date when the user was created
    /// </summary>
    public DateTime CreateAt { get; private set; }

    /// <summary>
    ///  Authorization by user
    /// </summary>
    public UserAuthorization Authorization { get; private set; }

    /// <summary>
    /// Used for EF Core
    /// </summary>
    private User() { }

    /// <summary>
    /// Constructor used to set new User
    /// </summary>
    /// <param name="name">User's full name</param>
    /// <param name="email">User's email address will be used for authentication</param>
    /// <param name="password">Hashed password </param>
    public User(string name, Email email, Password password)
    {
        Id = Guid.NewGuid();
        SetName(name);
        Email = email;
        Password = password;
        CreateAt = DateTime.UtcNow;
    }

    public User(Guid id, string name, Email email, Password password)
    {
        Id = id;
        SetName(name);
        Email = email;
        Password = password;
        CreateAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validation for user's name
    /// </summary>
    /// <param name="name">User's full name</param>
    /// <exception cref="ArgumentException">Throw if the user's name is invalid</exception>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("User cannot be empty.", nameof(name));
        }

        if (name.Length > 100)
        {
            throw new ArgumentException("User name is to long", nameof(name));
        }
        Name = name.Trim();
    }

    public void SetEmail(string email)
    {
        var validate = new Email(email);
        Email = validate.Value.Trim();
    }

    public void SetPermission(AuthorizationPermissions permissions)
    {
        Authorization = new UserAuthorization(Id, permissions);
    }
}
