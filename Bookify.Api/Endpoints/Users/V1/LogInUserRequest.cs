namespace Bookify.Api.Endpoints.Users.V1;

public sealed record LogInUserRequest(string Email, string Password);