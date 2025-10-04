using ServerSentEventsDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCors(builder.Configuration);
builder.Services.AddOpenApiServices();
builder.Services.AddEventStreaming();
builder.Services.ConfigureServices();

var app = builder.Build();

app.UseOpenApiWithSwagger();

app.UseCors("CorsPolicy");

app.MapSseEndpoints();

app.Run();