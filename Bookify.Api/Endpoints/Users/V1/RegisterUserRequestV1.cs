namespace Bookify.Api.Endpoints.Users.V1;

public sealed record RegisterUserRequestV1(string Email, string FirstName, string LastName, string Password);