using FCG_MS_Users.Application.Dtos;
using FCG_MS_Users.Application.Services;
using FCG_MS_Users.Domain.Entities;
using FCG_MS_Users.Domain.Exceptions;
using FCG_MS_Users.Domain.Interfaces;
using FCG_MS_Users.Domain.ValueObjects;
using FCG_MS_Users.Infrastructure.ExternalClients.Interfaces;
using Moq;
using Xunit;

namespace FCG_MS_Users.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserAuthorizationRepository> _userAuthorizationRepositoryMock;
    private readonly Mock<IUserNotificationClient> _userNotificationClient;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userAuthorizationRepositoryMock = new Mock<IUserAuthorizationRepository>();
        _userNotificationClient = new Mock<IUserNotificationClient>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _userAuthorizationRepositoryMock.Object,
            _userNotificationClient.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldCreateUser_WhenValid()
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "ValidPass1!",
            ConfirmationPassword = "ValidPass1!"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync((Domain.Entities.User)null);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.User>()))
            .Returns(Task.CompletedTask);

        _userAuthorizationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserAuthorization>()))
            .Returns(Task.CompletedTask);

        var result = await _userService.RegisterUserAsync(dto);

        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
        _userAuthorizationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserAuthorization>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldThrow_WhenEmailExists()
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "existing@example.com",
            Password = "ValidPass1!",
            ConfirmationPassword = "ValidPass1!"
        };

        var existingUser = new Domain.Entities.User("Existing", new Email(dto.Email), new Password(dto.Password));

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(existingUser);

        await Assert.ThrowsAsync<DomainException>(() =>
            _userService.RegisterUserAsync(dto));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenExists()
    {

        const string email = "test@example.com";
        var expectedUser = new Domain.Entities.User("Test", new Email(email), new Password("ValidPass1!"));

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(expectedUser);

        var result = await _userService.GetUserByEmailAsync(email);

        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        const string email = "nonexistent@example.com";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync((Domain.Entities.User)null);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchUsersAsync_ShouldReturnUser_WhenExists()
    {

        const string email = "test@example.com";
        const string name = "Test";
        var userList = new List<Domain.Entities.User>();
        var expectedUser = new Domain.Entities.User("Test", new Email(email), new Password("ValidPass1!"));
        expectedUser.SetPermission(default);
        userList.Add(expectedUser);

        _userRepositoryMock.Setup(x => x.SearchUsersAsync(email, name))
            .ReturnsAsync(userList);

        var result = await _userService.SearchUsersAsync(email, name);

        Assert.Contains(result, x =>
            x.Name.Equals(expectedUser.Name, StringComparison.Ordinal) &&
            x.Email.Equals(expectedUser.Email, StringComparison.Ordinal));
    }

    [Fact]
    public async Task SearchUsersAsync_WhenSendOnlyEmail_ShouldReturnUser_WhenExists()
    {

        const string email = "test@example.com";
        var userList = new List<Domain.Entities.User>();
        var expectedUser = new Domain.Entities.User("Test", new Email(email), new Password("ValidPass1!"));
        expectedUser.SetPermission(default);
        userList.Add(expectedUser);

        _userRepositoryMock.Setup(x => x.SearchUsersAsync(email, default))
            .ReturnsAsync(userList);

        var result = await _userService.SearchUsersAsync(email, default);

        Assert.Contains(result, x =>
            x.Name.Equals(expectedUser.Name, StringComparison.Ordinal) &&
            x.Email.Equals(expectedUser.Email, StringComparison.Ordinal));
    }

    [Fact]
    public async Task SearchUsersAsync_WhenSendOnlyName_ShouldReturnUser_WhenExists()
    {

        const string name = "Test";
        var userList = new List<Domain.Entities.User>();
        var expectedUser = new Domain.Entities.User("Test", new Email("test@example.com"), new Password("ValidPass1!"));
        expectedUser.SetPermission(default);
        userList.Add(expectedUser);

        _userRepositoryMock.Setup(x => x.SearchUsersAsync(default, name))
            .ReturnsAsync(userList);

        var result = await _userService.SearchUsersAsync(default, name);

        Assert.Contains(result, x =>
            x.Name.Equals(expectedUser.Name, StringComparison.Ordinal) &&
            x.Email.Equals(expectedUser.Email, StringComparison.Ordinal));
    }

    [Fact]
    public async Task SearchUsersAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        const string email = "nonexistent@example.com";
        const string name = "nonexistent";

        _userRepositoryMock.Setup(x => x.SearchUsersAsync(email, name))
            .ReturnsAsync(new List<Domain.Entities.User>());

        // Act
        var result = await _userService.SearchUsersAsync(email, name);

        // Assert
        Assert.True(result.Count == 0);
    }
    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenDtoIsNull()
    {
        // Act
        var result = await _userService.UpdateAsync(null);

        // Assert
        Assert.Equal("Request invalid is not allowed to be null", result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var dto = new UpdateUserDto { UserId = Guid.NewGuid(), Name = "Teste", Email = "teste@email.com" };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(dto.UserId))
            .ReturnsAsync((Domain.Entities.User)null);

        // Act
        var result = await _userService.UpdateAsync(dto);

        // Assert
        Assert.Equal("UserId does not exist", result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        // Arrange
        var dto = new UpdateUserDto { UserId = Guid.NewGuid(), Name = "Novo", Email = "existe@email.com" };
        var existingUser = new Domain.Entities.User("Usuário", new Email("existe@email.com"), new Password("Senha123!"));

        _userRepositoryMock.Setup(r => r.GetByIdAsync(dto.UserId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(r => r.SearchUsersAsync(dto.Email, null))
            .ReturnsAsync(new List<Domain.Entities.User> { new Domain.Entities.User("Outro", new Email("existe@email.com"), new Password("Senha456!")) });

        // Act
        var result = await _userService.UpdateAsync(dto);

        // Assert
        Assert.Equal("Email indicated already exists in the database", result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSuccessfully()
    {
        // Arrange
        var dto = new UpdateUserDto { UserId = Guid.NewGuid(), Name = "Teste Atualizada", Email = "teste@email.com" };
        var existingUser = new Domain.Entities.User("Le", new Email("Teste@antigo.com"), new Password("Senha123!"));

        _userRepositoryMock.Setup(r => r.GetByIdAsync(dto.UserId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(r => r.SearchUsersAsync(dto.Email, null))
            .ReturnsAsync(new List<Domain.Entities.User>()); // email não existe

        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(existingUser.Id))
            .ReturnsAsync(existingUser); // opcional

        // Act
        var result = await _userService.UpdateAsync(dto);

        // Assert
        Assert.Equal("Usuario Atualizado com sucesso", result);
        Assert.Equal("Teste Atualizada", existingUser.Name);
        Assert.Equal("teste@email.com", existingUser.Email);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Domain.Entities.User("Letícia", new Email("le@email.com"), new Password("Senha123!"));

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        await _userService.DeleteAsync(userId);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotCallRepository_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User)null!);

        // Act
        await _userService.DeleteAsync(userId);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}
