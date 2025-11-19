namespace Bookify.Api.Endpoints;

internal interface IEndpointBase
{
    static abstract string Tag { get; }
}

internal interface IEndpoints : IEndpointBase
{
    static abstract void DefineEndpoints(IEndpointRouteBuilder app);
}

internal interface IVersionedEndpoints : IEndpointBase
{
    static abstract int MajorVersion { get; }

    static abstract int MinorVersion { get; }

    static abstract void DefineEndpoints(IVersionedEndpointRouteBuilder app);
}