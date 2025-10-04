namespace ServerSentEventsDemo;

public record BroadcastMessage<TDto,TType>(TDto Dto, TType SseType) where TDto : IBaseDto where TType : ISseType;

public interface ISseType;

public record CreatedMessageType: ISseType;
public record UpdatedMessageType: ISseType;
public record DeletedMessageType: ISseType;