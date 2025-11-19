namespace Bookify.Api.Endpoints;

internal static class Versions
{
    internal static int V1 => 1;

    internal static int V2 => 2;

    internal static int[] All => [V1, V2];

    internal static string[] AllAsStrings=> [..All.Select(verionNumber => $"v{verionNumber}")];
}