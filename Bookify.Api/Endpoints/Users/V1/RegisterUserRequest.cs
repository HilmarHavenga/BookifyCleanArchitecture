namespace Bookify.Api.Endpoints.Users.V1;

public sealed record RegisterUserRequest(string Email, string FirstName, string LastName, string Password);