namespace ServerSentEventsDemo;

public record ServerResponse(string Name, string Status): IBaseDto;