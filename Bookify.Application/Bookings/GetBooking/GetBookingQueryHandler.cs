namespace Bookify.Application.Bookings.GetBooking;

internal sealed class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserContext _userContext;


    public GetBookingQueryHandler(IBookingRepository bookingRepository, IUserContext userContext)
    {
        _bookingRepository = bookingRepository;
        _userContext = userContext;
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if(booking is null || booking.UserId != _userContext.UserId)
        {
            return Result.Failure<BookingResponse>(BookingErrors.NotFound);
        }

        return Result.Success(booking.Adapt<BookingResponse>());

        throw new NotImplementedException();
    }
}