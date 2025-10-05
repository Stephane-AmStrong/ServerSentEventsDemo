
namespace ServerSentEventsDemo;

public class Processor(IEventStreamingService<ServerResponse> serverEventStreaming, IEventStreamingService<ClientResponse> clientEventStreaming) : BackgroundService
{
    private const int MillisecondsDelay = 1000;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var i = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(MillisecondsDelay, stoppingToken);

            var server = new ServerResponse("ED209", $"Countdown at {DateTime.Now}");
            var client = new ClientResponse("Robocop", $"Countdown at {DateTime.Now}");

            if (i++ % 2 == 0)
            {
                await clientEventStreaming.BroadcastAsync(new BroadcastMessage<ClientResponse>(client, BroadcastMessageType.Updated), stoppingToken);
            }
            else
            {
                await serverEventStreaming.BroadcastAsync(new BroadcastMessage<ServerResponse>(server, BroadcastMessageType.Created), stoppingToken);
            }
        }
    }
}

