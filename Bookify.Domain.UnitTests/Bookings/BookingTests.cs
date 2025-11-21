namespace Bookify.Domain.UnitTests.Bookings;

public class BookingTests : BaseTest
{
    //This is not complete. Just an example

    [Fact]
    public void Reserve_Should_SetLastBookedOnUtc()
    {
        //Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Usd);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();
        var utcNow = DateTime.UtcNow;

        //Act
        _ = Booking.Reserve(apartment, user.Id, period, utcNow, pricingService);

        //Assert
        apartment.LastBookedOnUtc.Should().Be(utcNow);
    }

    [Fact]
    public void Reserve_Should_RaiseBookingReservedDomainEvent()
    {
        //Arrange
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);
        var price = new Money(10.0m, Currency.Usd);
        var period = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
        var apartment = ApartmentData.Create(price);
        var pricingService = new PricingService();

        //Act
        var booking = Booking.Reserve(apartment, user.Id, period, DateTime.UtcNow, pricingService);

        //Assert
        var domainEvent = AssertDomainEventWasPublished<BookingReservedDomainEvent>(booking);

        domainEvent.BookingId.Should().Be(booking.Id);
    }
}