namespace Bookify.Api.Extensions;

internal static class EndpointExtensions
{
    public static void UseEndpoints<TMarker>(this IEndpointRouteBuilder app)
    {
        var endpointTypes = GetEndpointTypesFromAssembly<TMarker>(typeof(TMarker)).ToList();

        foreach (TypeInfo endpointType in endpointTypes)
        {
            var type = endpointType.AsType();

            if (typeof(IVersionedEndpoints).IsAssignableFrom(type))
            {
                endpointType
                    .GetMethod(nameof(IVersionedEndpoints.DefineEndpoints))
                    ?.Invoke(null, [app.NewVersionedApi(endpointType.Name)]);

                continue;
            }

            if (typeof(IEndpoints).IsAssignableFrom(type))
            {
                endpointType
                    .GetMethod(nameof(IEndpoints.DefineEndpoints))
                    ?.Invoke(null, [app]);

                continue;
            }

            throw new NotImplementedException($"Endpoint type {endpointType.Name} does not implement a supported interface.");
        }
    }

    internal static IEnumerable<TypeInfo> GetEndpointTypesFromAssembly<TMarker>(Type typeMarker)
    {
        return typeMarker.Assembly.DefinedTypes
            .Where(type => !type.IsAbstract && !type.IsInterface && typeof(IEndpointBase).IsAssignableFrom(type));
    }
}