using System.Threading.Channels;

namespace ServerSentEventsDemo;

public interface IEventStreamingService<TDto> where TDto : IBaseDto
{
    ValueTask BroadcastAsync(BroadcastMessage<TDto> evt, CancellationToken cancellationToken);
    
    ChannelReader<BroadcastMessage<TDto>> Events { get; }
}