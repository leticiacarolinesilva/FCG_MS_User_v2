namespace FCG_MS_Users.Domain.ValueObjects;

/// <summary>
/// Represents an email address with validation
/// </summary>
public class Email
{
    public string Value { get; }

    public Email(string value)
    {
        Value = value.Trim().ToLower();
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string email) => new Email(email);

}