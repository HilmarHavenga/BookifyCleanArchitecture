using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;

namespace Bookify.Api.FunctionalTests.Users;

public class GetLoggedInUser : BaseFunctionalTest
{
    public GetLoggedInUser(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccessTokenIsMissing()
    {
        //Act
        var response = await HttpClient.GetAsync("api/v1/users/me");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAccessTokenIsNotMissing()
    {
        //Arrange
        var accessToken = await GetAccessToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme, accessToken);

        //Act
        var response = await HttpClient.GetFromJsonAsync<string>("api/v1/users/me");

        //Assert
        response.Should().NotBeEmpty();
    }
}