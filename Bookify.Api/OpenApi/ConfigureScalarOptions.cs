namespace Bookify.Api.OpenApi;

public sealed class ConfigureScalarOptions : IConfigureNamedOptions<ScalarOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureScalarOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(string? name, ScalarOptions options)
    {
        Configure(options);
    }

    public void Configure(ScalarOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.AddDocument(
                description.GroupName,
                $"Bookify.Api v{description.ApiVersion}",
                $"/openapi/{description.GroupName}.json");
        }
    }
}