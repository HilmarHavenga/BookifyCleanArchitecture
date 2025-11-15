namespace Bookify.Domain.Reviews.Events;

public sealed record ReviewCreatedDomainEvent(Guid BookingId) : IDomainEvent;