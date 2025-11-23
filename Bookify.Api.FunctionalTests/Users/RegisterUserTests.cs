using Bookify.Api.Endpoints.Users.V1;
using Bookify.Domain.Users;

namespace Bookify.Api.FunctionalTests.Users;

public class RegisterUserTests : BaseFunctionalTest
{
    public RegisterUserTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnOk_WhenRequestIsValid()
    {
        //Arrange
        var request = new RegisterUserRequestV1("create@test.com", "First", "Last", "12345");

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("", "first", "last", "12345")]
    [InlineData("test.com", "first", "last", "12345")]
    [InlineData("@test.com", "first", "last", "12345")]
    [InlineData("test@", "first", "last", "12345")]
    [InlineData("test@test.com", "", "last", "12345")]
    [InlineData("test@test.com", "first", "", "12345")]
    [InlineData("test@test.com", "first", "last", "")]
    [InlineData("test@test.com", "first", "last", "1")]
    [InlineData("test@test.com", "first", "last", "12")]
    [InlineData("test@test.com", "first", "last", "123")]
    [InlineData("test@test.com", "first", "last", "1234")]

    public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid(string email, string firstName, string lastName, string password)
    { 
        // Arrange
        var request = new RegisterUserRequestV1(email, firstName, lastName, password);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}