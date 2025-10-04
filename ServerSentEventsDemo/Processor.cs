
namespace ServerSentEventsDemo;

public class Processor(IEventStreamingService<ServerResponse> eventStreaming) : BackgroundService
{
    private const int MillisecondsDelay = 1000;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(MillisecondsDelay, stoppingToken);

            var server = new ServerResponse("ED209", $"Countdown at {DateTime.Now}");
            var messageType = new CreatedMessageType();
            
            await eventStreaming.PublishAsync(new BroadcastMessage<ServerResponse, CreatedMessageType>(server, messageType), stoppingToken);
        }
    }
}

