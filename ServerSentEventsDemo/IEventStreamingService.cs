using System.Threading.Channels;

namespace ServerSentEventsDemo;

public interface IEventStreamingService<TDto> where TDto : IBaseDto
{
    ValueTask PublishAsync<TType>(BroadcastMessage<TDto, TType> evt, CancellationToken cancellationToken = default)
        where TType : ISseType;
    
    ChannelReader<BroadcastMessage<TDto, ISseType>> Events { get; }
}