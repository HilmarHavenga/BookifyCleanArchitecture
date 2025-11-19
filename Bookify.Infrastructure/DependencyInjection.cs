using AuthenticationOptions = Bookify.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Bookify.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Bookify.Application.Abstractions.Authentication.IAuthenticationService;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IEmailService, EmailService>();

        AddPersistence(services, configuration);
        AddAuthentication(services, configuration);
        AddAuthorization(services);
        AddCaching(services, configuration);
        AddHealthChecks(services, configuration);

        return services;
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer();

        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cache") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Cache")!)
            .AddUrlGroup(new Uri(configuration["KeyCloak:BaseUrl"]!), HttpMethod.Get, "keycloak");
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //This throws an exception on clean startup 
        dbContext.Database.Migrate();
    }

    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext.Set<Apartment>().Any())
        {
            return;
        }

        var faker = new Faker();

        List<Apartment> apartments = [];

        for (var i = 0; i < 100; i++)
        {
            Address address = new(
                faker.Address.Country(),
                faker.Address.State(),
                faker.Address.ZipCode(),
                faker.Address.City(),
                faker.Address.StreetAddress());

            Currency currency = Currency.FromCode("USD");
            Name name = new(faker.Company.CompanyName());
            Description description = new("Amazing view");
            Money pricePerNight = new(faker.Random.Decimal(50, 1000), currency);
            Money cleaningFee = new(faker.Random.Decimal(25, 200), currency);
            List<Amenity> amenities = [Amenity.Parking, Amenity.MountainView];

            Apartment apartment = new(Guid.NewGuid(), name, description, address, pricePerNight, cleaningFee, amenities);

            apartments.Add(apartment);
        }

        dbContext.AddRange(apartments);
        dbContext.SaveChanges();
    }
}