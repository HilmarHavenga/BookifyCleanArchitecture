using Bogus;
using Microsoft.AspNetCore.Builder;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IEmailService, EmailService>();

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

        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    public static void EnsureCleanDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureDeleted();
    }

    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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