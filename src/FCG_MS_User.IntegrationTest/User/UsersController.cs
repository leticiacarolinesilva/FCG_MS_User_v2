using FCG_MS_Users.Application.Dtos;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FCG_MS_Users.IntegrationTest.User;

public class UsersController : BaseIntegrationTests
{
    private const string BaseUrl = "http://localhost:5209/api/User";

    [Fact]
    public async Task RegisterUser_ShouldReturnCreated_WhenValid()
    {
        var request = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "ValidPass1!",
            ConfirmationPassword = "ValidPass1!"
        };

        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = response.Headers.Location;
        Assert.NotNull(location);
    }

    [Theory]
    [InlineData("Test User", "not-an-email", "ValidPass1!")]
    [InlineData("Test User", "test@test.com", "not-an-password")]
    [InlineData("", "test@test.com", "ValidPass1!")]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenNameOrEmailOrPasswordAreInvalid(string name, string email, string password)
    {
        var invalidUser = new RegisterUserDto
        {
            Name = name,
            Email = email,
            Password = password,
            ConfirmationPassword = password
        };

        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", invalidUser);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenThereIsAlreadyUser()
    {
        var request = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "ValidPass1!",
            ConfirmationPassword = "ValidPass1!"
        };

        await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

        //TODO improve the return to show Notification instead of exceptions
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }

    [Fact]
    public async Task RegisterUser_ShouldReturnNotFound_WhenIsNotRegisteredYet()
    {
        var email = "notexist@test.com";

        var response = await HttpClient.GetAsync($"{BaseUrl}/?email={email}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var location = response.Headers.Location;
        Assert.Null(location);
    }

    [Fact]
    public async Task GetUser_WhenSendEmailAndName_ShouldReturnBadRequest_WhenThereIsAlreadyUser()
    {
        var request = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "ValidPass1!",
            ConfirmationPassword = "ValidPass1!"
        };

        var responseCreation = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

        var response = await HttpClient.GetAsync($"{BaseUrl}?email={request.Email}&name={request.Name}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.Created, responseCreation.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<ResponseUserDto>>(content);

        Assert.Contains(users, x => x.Name == request.Name && x.Email == request.Email);
    }

    [Fact]
    public async Task GetUser_WhenSendOnlyEmail_ShouldReturnBadRequest_WhenThereIsAlreadyUser()
    {
        var request = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "ValidPass1!",
            ConfirmationPassword = "ValidPass1!"
        };

        var responseCreation = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

        var response = await HttpClient.GetAsync($"{BaseUrl}?email={request.Email}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.Created, responseCreation.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var users = JsonConvert.DeserializeObject<List<ResponseUserDto>>(content);

        Assert.Contains(users, x => x.Name == request.Name && x.Email == request.Email);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateSuccessfully_WhenUserExists()
    {
        // Arrange
        var registerDto = new RegisterUserDto
        {
            Name = "Letícia Original",
            Email = "leticia.original@email.com",
            Password = "Senha123!",
            ConfirmationPassword = "Senha123!"
        };

        var createResponse = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", registerDto);
        createResponse.EnsureSuccessStatusCode();

        // Busca o usuário criado para pegar o Id
        var getResponse = await HttpClient.GetAsync($"{BaseUrl}?email={registerDto.Email}");
        getResponse.EnsureSuccessStatusCode();

        var getContent = await getResponse.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<ResponseUserDto>>(getContent);
        var user = users.FirstOrDefault();

        Assert.NotNull(user);

        // Act: atualiza o nome e email
        var updateDto = new UpdateUserDto
        {
            UserId = user.Id,
            Name = "Letícia Atualizada",
            Email = "leticia.atualizada@email.com"
        };

        var updateResponse = await HttpClient.PutAsJsonAsync(BaseUrl, updateDto);

        // Assert: verifica se atualizou com sucesso
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedCheck = await HttpClient.GetAsync($"{BaseUrl}?email={updateDto.Email}");
        updatedCheck.EnsureSuccessStatusCode();

        var updatedContent = await updatedCheck.Content.ReadAsStringAsync();
        var updatedUsers = JsonConvert.DeserializeObject<List<ResponseUserDto>>(updatedContent);

        var updatedUser = updatedUsers.FirstOrDefault();
        Assert.NotNull(updatedUser);
        Assert.Equal(updateDto.Name, updatedUser.Name);
        Assert.Equal(updateDto.Email, updatedUser.Email);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
    {
        // Arrange – cria um usuário
        var registerDto = new RegisterUserDto
        {
            Name = "Usuário Deletável",
            Email = "deletar@email.com",
            Password = "Senha123!",
            ConfirmationPassword = "Senha123!"
        };

        var createResponse = await HttpClient.PostAsJsonAsync($"{BaseUrl}/register", registerDto);
        createResponse.EnsureSuccessStatusCode();

        // Busca o usuário para pegar o ID
        var getResponse = await HttpClient.GetAsync($"{BaseUrl}?email={registerDto.Email}");
        getResponse.EnsureSuccessStatusCode();

        var getContent = await getResponse.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<ResponseUserDto>>(getContent);
        var user = users.FirstOrDefault();

        Assert.NotNull(user);

        // Act – envia o DELETE com userId na query
        var deleteResponse = await HttpClient.DeleteAsync($"{BaseUrl}?userId={user.Id}");

        // Assert – espera NoContent (204)
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verifica se usuário foi realmente deletado
        var verifyResponse = await HttpClient.GetAsync($"{BaseUrl}?email={registerDto.Email}");

        Assert.Equal(HttpStatusCode.NotFound, verifyResponse.StatusCode);
    }
}
