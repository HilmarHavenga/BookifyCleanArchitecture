namespace Bookify.Api.Endpoints;

internal interface IEndpoints
{
    static abstract void DefineEndpoints(IVersionedEndpointRouteBuilder app);
}