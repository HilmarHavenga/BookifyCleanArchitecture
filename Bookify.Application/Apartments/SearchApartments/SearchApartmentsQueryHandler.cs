namespace Bookify.Application.Apartments.SearchApartments;

internal sealed class SearchApartmentsQueryHandler : IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
{
   
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBookingRepository _bookingRepository;
    private static readonly BookingStatus[] _activeBookingStatuses =
    [
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Completed
    ];

    public SearchApartmentsQueryHandler(IApartmentRepository apartmentRepository, IBookingRepository bookingRepository)
    {
        _apartmentRepository = apartmentRepository;
        _bookingRepository = bookingRepository;

        TypeAdapterConfig<Apartment, ApartmentResponse>.NewConfig()
            .Map(dest => dest.Name, src => src.Name.Value)
            .Map(dest => dest.Description, src => src.Description.Value)
            .Map(dest => dest.Price, src => src.PricePerNight.Amount)
            .Map(dest => dest.Currency, src => src.PricePerNight.Currency.Code);
    }

    public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
    {
        if(request.StartDate > request.EndDate)
        {
            return new List<ApartmentResponse>();
        }

        var bookedApartments = _bookingRepository.DbSet().Where(x =>
        x.Duration.Start <= request.EndDate &&
        x.Duration.End >= request.StartDate &&
        !_activeBookingStatuses.Contains(x.Status)).Select(x => x.ApartmentId).ToList();

        List<Apartment> availableAppartments = [.. _apartmentRepository.DbSet().Where(x => !bookedApartments.Contains(x.Id))];

        return Result.Success(availableAppartments.Adapt<IReadOnlyList<ApartmentResponse>>());
    }
}