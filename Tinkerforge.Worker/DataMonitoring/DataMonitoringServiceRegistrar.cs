namespace Tinkerforge.Worker.DataMonitoring;

public static class DataMonitoringServiceRegistrar
{
    public static IServiceCollection AddDataMonitoringServices(this IServiceCollection services)
    {
        services.AddHostedService<DataMonitoringWorker>();
        services.AddTransient<DataMonitoringService>();

        return services;
    }
}