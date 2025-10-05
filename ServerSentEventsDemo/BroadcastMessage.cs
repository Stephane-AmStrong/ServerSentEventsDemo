namespace ServerSentEventsDemo;

public record BroadcastMessage<TDto>(TDto Dto, BroadcastMessageType MessageType) where TDto : IBaseDto;

public enum BroadcastMessageType
{
    Created,
    Updated,
    Deleted
}