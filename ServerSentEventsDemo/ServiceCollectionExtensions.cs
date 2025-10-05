namespace ServerSentEventsDemo;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[] allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                //builder.WithOrigins(allowedOrigins)
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

        });
    }
    
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddHostedService<Processor>();
    }
    
    public static void AddOpenApiServices(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
    }

    public static void UseOpenApiWithSwagger(this WebApplication app)
    {
        app.MapOpenApi();

        // Configure OpenAPI mapping and Swagger UI
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Icc Web API");
            options.RoutePrefix = string.Empty;
        });
    }
    
    public static IServiceCollection AddEventStreaming(this IServiceCollection services)
    {
        // Un service par type de DTO
        services.AddSingleton<IEventStreamingService<ServerResponse>, EventStreamingService<ServerResponse>>();
        services.AddSingleton<IEventStreamingService<ClientResponse>, EventStreamingService<ClientResponse>>();
        
        return services;
    }
    
}