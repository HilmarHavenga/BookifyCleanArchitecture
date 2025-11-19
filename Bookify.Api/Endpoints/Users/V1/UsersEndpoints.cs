namespace Bookify.Api.Endpoints.Users.V1;

public class UsersEndpoints : IVersionedEndpoints
{
    public static string Tag => $"{nameof(UsersEndpoints)}";
    public static int MajorVersion => 1;
    public static int MinorVersion => 0;

    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/users").HasApiVersion(MajorVersion, MinorVersion);

        versioned.MapPost("/register", RegisterUser)
            .WithName("RegisterUser")
            .Accepts<RegisterUserRequestV1>(EndpointDefaults.CONTENT_TYPE)
            .Produces(200)
            .Produces<Error>(400)
            .WithTags(Tag).AllowAnonymous();

        versioned.MapPost("/login", LoginUser)
            .WithName("LoginUser")
            .Accepts<LogInUserRequestV1>(EndpointDefaults.CONTENT_TYPE)
            .Produces(200)
            .Produces(401)
            .WithTags(Tag).AllowAnonymous();

        versioned.MapPost("/me", GetLoggedInUser)
            .WithName("GetLoggedInUser")
            .Produces(200)
            .Produces(401)
            .WithTags(Tag).RequireAuthorization(Policies.REGISTERED_ROLE_POLICY);
    }

    internal static async Task<IResult> RegisterUser(ISender sender, RegisterUserRequestV1 request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        var result = await sender.Send(command, cancellationToken);

        if(result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok(result.Value);
    }

    internal static IResult GetLoggedInUser(ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok("You are logged in silly");
    }

    internal static async Task<IResult> LoginUser(ISender sender, LogInUserRequestV1 request, CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(
            request.Email,
            request.Password);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Value);
    }
}
