using System.Threading.Channels;

namespace ServerSentEventsDemo;

public class EventStreamingService<TDto> : IEventStreamingService<TDto> where TDto : IBaseDto
{
    private readonly Channel<BroadcastMessage<TDto, ISseType>> _channel = Channel.CreateUnbounded<BroadcastMessage<TDto, ISseType>>(new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = true,
        AllowSynchronousContinuations = true
    });

    public ValueTask PublishAsync<TType>(BroadcastMessage<TDto, TType> evt, CancellationToken cancellationToken = default)
        where TType : ISseType
    {
        var baseEvent = new BroadcastMessage<TDto, ISseType>(evt.Dto, evt.SseType);
        return _channel.Writer.WriteAsync(baseEvent, cancellationToken);
    }

    public ChannelReader<BroadcastMessage<TDto, ISseType>> Events => _channel.Reader;
}