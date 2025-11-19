var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

foreach (var versionString in Versions.AllAsStrings)
{
    builder.Services.AddOpenApi(versionString);
}
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(
    options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    })
    .EnableApiVersionBinding();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.REGISTERED_ROLE_POLICY, policy => policy.RequireRole(Roles.REGISTERED));

builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureOptions<ConfigureScalarOptions>();

//Prefer to use the health check packages in infrastructure for standard checks
//But here is a custom health check example if needed
//builder.Services.AddHealthChecks().AddCheck<CustomSqlHealthCheck>("custom-sql");

var app = builder.Build();

app.UseCustomExceptionHandler();

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints<IApiMarker>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.ApplyMigrations();
    app.SeedData();
}

app.Run();