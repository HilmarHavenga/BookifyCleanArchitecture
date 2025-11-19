namespace Bookify.Api.Endpoints.Apartments.V1;

public sealed class ApartmentEndpoints : IEndpoints
{
    public static string Tag => $"{nameof(ApartmentEndpoints)}";

    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/apartments").HasApiVersion(Versions.V1).RequireAuthorization();

        versioned.MapGet("/", SearchApartments)
            .WithName("SearchApartments")
            .Produces<IReadOnlyList<ApartmentResponse>>(200)
            .WithTags(Tag);
    }

    internal static async Task<IResult> SearchApartments(ISender sender, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var query = new SearchApartmentsQuery(startDate, endDate);

        Result<IReadOnlyList<ApartmentResponse>> result = await sender.Send(query, cancellationToken);

        return Results.Ok(result.Value);
    }
}