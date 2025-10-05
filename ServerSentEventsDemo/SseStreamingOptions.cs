using System.Text.Json;

namespace ServerSentEventsDemo;

public record SseStreamingOptions(JsonSerializerOptions? JsonOptions);