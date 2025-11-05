using FCG_MS_Users.Domain.ValueObjects;
using Xunit;

namespace FCG_MS_Users.Tests;

public class UsersTests
{
    private readonly Email _validEmail = new Email("test@example.com");
    private readonly Password _validPassword = new Password("ValidPass1!");

    [Fact]
    public void User_ShouldCreate_WithValidParameters()
    {
        const string name = "Test User";

        var user = new Domain.Entities.User(name, _validEmail, _validPassword);

        Assert.Equal(name, user.Name);
        Assert.Equal(_validEmail.Value, user.Email.Value);
        Assert.True(user.CreateAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void User_ShouldThrow_WhenNameIsEmpty(string name)
    {

        Assert.Throws<ArgumentException>(() =>
            new Domain.Entities.User(name, _validEmail, _validPassword));
    }

    [Fact]
    public void User_ShouldThrow_WhenNameTooLong()
    {

        var longName = new string('a', 101); // 101 characters

        Assert.Throws<ArgumentException>(() =>
            new Domain.Entities.User(longName, _validEmail, _validPassword));
    }
    [Fact]
    public void SetName_ShouldUpdate_WhenValid()
    {
        var user = new Domain.Entities.User("Old Name", _validEmail, _validPassword);
        const string newName = "New Name";

        user.SetName(newName);

        Assert.Equal(newName, user.Name);
    }

}
