namespace Bookify.Api.Endpoints;

internal interface IEndpoints
{
    static abstract string ContentType { get; }

    static abstract string Tag { get; }
    
    static abstract int MajorVersion { get; }
    
    static abstract int MinorVersion { get; }

    static abstract void DefineEndpoints(IVersionedEndpointRouteBuilder app);
}