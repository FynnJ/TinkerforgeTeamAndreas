using RestEase;
using Tinkerforge.Worker.NotificationService.TelegramClient;

namespace Tinkerforge.Worker.NotificationService;

public static class NotificationServiceRegistrar
{
    public static IServiceCollection AddNotificationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<NotificationWorker>();
        services.AddSingleton<TelegramConfiguration>(_ =>
            configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>()
            ?? new TelegramConfiguration());

        services.AddTransient<ITelegramClient>(serviceProvider =>
            RestClient.For<ITelegramClient>(serviceProvider.GetRequiredService<TelegramConfiguration>().BaseUrl));

        services.AddTransient<ITelegramService, TelegramService>();

        services.AddTransient<NotificationService>();
        
        return services;
    }
}