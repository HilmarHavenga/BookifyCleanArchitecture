namespace Bookify.Application.IntegrationTests.Bookings;

public class ConfirmBookingTests : BaseIntegrationTest
{
    private static readonly Guid BookingId = Guid.NewGuid();

    public ConfirmBookingTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    //[Fact]
    //public async Task GetBooking_ShouldReturnFailure_WhenBookingIsNotFound()
    //{
    //    //Arrange
    //    var query = new ConfirmBookingCommand(BookingId);

    //    //Act
    //    var result = await Sender.Send(query);

    //    //Assert
    //    result.Error.Should().Be(BookingErrors.NotFound);
    //}
}
