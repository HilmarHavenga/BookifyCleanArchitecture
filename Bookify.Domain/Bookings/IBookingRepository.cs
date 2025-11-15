namespace Bookify.Domain.Bookings;

public interface IBookingRepository : IRepository<Booking>
{
    Task<bool> IsOverlappingAsync(
        Apartment apartment,
        DateRange duration,
        CancellationToken cancellationToken = default);
}