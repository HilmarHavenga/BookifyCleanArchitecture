namespace Bookify.Application.Bookings.GetBooking;

internal sealed class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserContext _userContext;

    public GetBookingQueryHandler(IBookingRepository bookingRepository, IUserContext userContext)
    {
        _bookingRepository = bookingRepository;
        _userContext = userContext;

        TypeAdapterConfig<Booking, BookingResponse>.NewConfig()
        .Map(dest => dest.PriceAmount, src => src.PriceForPeriod.Amount)
        .Map(dest => dest.PriceCurrency, src => src.PriceForPeriod.Currency.Code)
        .Map(dest => dest.CleaningFeeCurrency, src => src.CleaningFee.Currency.Code)
        .Map(dest => dest.AmenitiesUpChargeCurrency, src => src.AmenitiesUpCharge.Currency.Code);
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if(booking is null || booking.UserId != _userContext.UserId)
        {
            return Result.Failure<BookingResponse>(BookingErrors.NotFound);
        }

        return Result.Success(booking.Adapt<BookingResponse>());
    }
}