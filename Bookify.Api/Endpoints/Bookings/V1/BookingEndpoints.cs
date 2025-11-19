namespace Bookify.Api.Endpoints.Bookings.V1;

public class BookingEndpoints : IVersionedEndpoints
{
    public static string Tag => $"{nameof(BookingEndpoints)}";
    public static int MajorVersion => 1;
    public static int MinorVersion => 0;

    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/bookings").HasApiVersion(MajorVersion, MinorVersion).RequireAuthorization(Policies.REGISTERED_ROLE_POLICY);

        versioned.MapGet("/", GetBooking)
            .WithName("GetBooking")
            .Produces<BookingResponse>(200)
            .Produces(404)
            .WithTags(Tag);

        versioned.MapPost("/", ReserveBooking)
            .WithName("ReserveBooking")
            .Accepts<ReserveBookingRequestV1>(EndpointDefaults.CONTENT_TYPE)
            .Produces<Guid>(200)
            .Produces<Error>(400)
            .WithTags(Tag);
    }


    internal static async Task<IResult> GetBooking(ISender sender, [FromQuery]Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBookingQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
    }


    internal static async Task<IResult> ReserveBooking(ISender sender, ReserveBookingRequestV1 request, CancellationToken cancellationToken)
    {
        var command = new ReserveBookingCommand(request.ApartmentId, request.UserId, request.StartDate, request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        if(result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.CreatedAtRoute(nameof(GetBooking), new { id = result.Value }, result.Value);
    }
}