using System.Threading.Channels;

namespace ServerSentEventsDemo;

public class EventStreamingService<TDto> : IEventStreamingService<TDto> where TDto : IBaseDto
{
    private readonly Channel<BroadcastMessage<TDto>> _channel = Channel.CreateUnbounded<BroadcastMessage<TDto>>(new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = true,
        AllowSynchronousContinuations = true
    });

    public ValueTask BroadcastAsync(BroadcastMessage<TDto> evt, CancellationToken cancellationToken)
    {
        var baseEvent = new BroadcastMessage<TDto>(evt.Dto, evt.MessageType);
        return _channel.Writer.WriteAsync(baseEvent, cancellationToken);
    }

    public ChannelReader<BroadcastMessage<TDto>> Events => _channel.Reader;
}