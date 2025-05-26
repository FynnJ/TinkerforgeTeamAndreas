namespace Tinkerforge.Worker.Authorization;

public static class AuthServiceRegister
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddHostedService<AuthWorker>();
        services.AddTransient<AuthService>();

        return services;
    }
}