namespace Bookify.Api.Endpoints.Users.V1;

public class UsersEndpoints : IEndpoints
{
    public static string ContentType => "application/json";
    public static string Tag => $"{nameof(UsersEndpoints)}";
    public static int MajorVersion => 1;
    public static int MinorVersion => 0;

    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/users").HasApiVersion(MajorVersion, MinorVersion);

        versioned.MapPost("/register", RegisterUser)
            .WithName("RegisterUser")
            .Produces(200)
            .Produces<Error>(400)
            .WithTags(Tag).AllowAnonymous();

        versioned.MapPost("/login", LoginUser)
            .WithName("LoginUser")
            .Produces(200)
            .Produces(401)
            .WithTags(Tag).AllowAnonymous();

        versioned.MapPost("/me", GetLoggedInUser)
            .WithName("GetLoggedInUser")
            .Produces(200)
            .Produces(401)
            .WithTags(Tag).RequireAuthorization(PolicyNames.RegisteredRolePolicy);
    }

    internal static async Task<IResult> RegisterUser(ISender sender, RegisterUserRequest request, CancellationToken cancellationToken)
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

    internal static async Task<IResult> LoginUser(ISender sender, LogInUserRequest request, CancellationToken cancellationToken)
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
