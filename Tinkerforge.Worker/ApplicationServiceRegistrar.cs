namespace Tinkerforge.Worker;

public static class ApplicationServiceRegistrar
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHostedService<Worker>();
        
        services.AddSingleton<IPConnection>(_ =>
        {
            var ipConnection = new IPConnection();
            ipConnection.Connect("172.20.10.242", 4223);
            return ipConnection;
        });

        return services;
    }
}