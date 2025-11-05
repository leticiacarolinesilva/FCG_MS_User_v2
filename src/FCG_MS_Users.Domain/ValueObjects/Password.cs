using FCG_MS_Users.Domain.Helper;

namespace FCG_MS_Users.Domain.ValueObjects;
/// <summary>
/// Represents a secure password with hashing and verification
/// </summary>
public sealed class Password
{
    public string HasedValue { get; private set; }

    /// <summary>
    /// Used for EF Core
    /// </summary>
    private Password() { }

    public Password(string rawPassword)
    {
        if (string.IsNullOrWhiteSpace(rawPassword))
            throw new ArgumentException("Password cannot be empty", nameof(rawPassword));

        HasedValue = PasswordHelper.HashPassword(rawPassword);
    }

    public bool Verify(string rawPassword)
    {
        return PasswordHelper.VerifyPassword(rawPassword, HasedValue);
    }
}