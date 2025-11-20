using Newtonsoft.Json;
using System.Data;

namespace Bookify.Infrastructure.Outbox;

internal sealed class ProcessOutboxMessagesService : BackgroundService
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<ProcessOutboxMessagesService> _logger;
    private readonly PeriodicTimer _timer;

    public ProcessOutboxMessagesService(IServiceScopeFactory serviceScopeFactory, IDateTimeProvider dateTimeProvider, IOptions<OutboxOptions> outboxOptions, ILogger<ProcessOutboxMessagesService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeProvider = dateTimeProvider;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
        _timer = new(TimeSpan.FromSeconds(_outboxOptions.IntervalInSeconds));
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background service started");

        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            DbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            var outboxMessages = await dbContext.Set<OutboxMessage>()
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(_outboxOptions.BatchSize)
                .ToListAsync(stoppingToken);

            foreach (var outboxMessage in outboxMessages)
            {
                Exception? exception = null;

                try
                {
                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, _jsonSerializerSettings)!;

                    await publisher.Publish(domainEvent, stoppingToken);

                    _logger.LogInformation("Successfully published event: {EventType}", outboxMessage.Content);
                }
                catch (Exception caughtException)
                {
                    _logger.LogError(caughtException, "Exception while processing outbox message {MessageId}", outboxMessage.Id);
                    exception = caughtException;
                }

                outboxMessage.SetProcessedOnUtc(_dateTimeProvider);

                if(exception is not null)
                    outboxMessage.SetError(exception.ToString());

                dbContext.Update(outboxMessage);
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}