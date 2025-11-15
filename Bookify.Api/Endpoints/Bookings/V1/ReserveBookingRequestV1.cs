namespace Bookify.Api.Endpoints.Bookings.V1;

public sealed record ReserveBookingRequestV1(Guid ApartmentId, Guid UserId, DateOnly StartDate, DateOnly EndDate);