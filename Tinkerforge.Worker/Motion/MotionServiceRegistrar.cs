namespace Tinkerforge.Worker.Motion;

public static class MotionServiceRegistrar
{
    public static IServiceCollection AddMotionServices(this IServiceCollection services)
    {
        services.AddHostedService<MotionWorker>();
        services.AddSingleton<MotionService>();

        return services;
    }
}