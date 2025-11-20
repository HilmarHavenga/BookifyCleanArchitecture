namespace Bookify.Infrastructure.Outbox;

internal sealed class OutboxMessage
{
    public OutboxMessage()
    {
    }

    public OutboxMessage(Guid id, DateTime occuredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occuredOnUtc;
        Type = type;
        Content = content;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }

    public string Type { get; init; }

    public string Content { get; init; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    internal void SetProcessedOnUtc(IDateTimeProvider dateTimeProvider) => ProcessedOnUtc = dateTimeProvider.UtcNow;

    internal void SetError(string error) => Error = error;
}