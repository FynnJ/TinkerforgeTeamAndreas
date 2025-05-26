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

        services.AddTransient<NotificationService>(serviceProvider =>
        {
            var ipConnection = serviceProvider.GetRequiredService<IPConnection>();
            var telegramService = serviceProvider.GetRequiredService<ITelegramService>();
            var temperatureThreshold = configuration["MonitoringThreshold:Temperature"] ?? "30";
            var humidityThreshold = configuration["MonitoringThreshold:Humidity"] ?? "60";

            return new NotificationService(
                ipConnection,
                telegramService,
                int.Parse(temperatureThreshold),
                int.Parse(humidityThreshold));
        });

        return services;
    }
}