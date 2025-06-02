namespace Tinkerforge.Worker.Authorization;

public static class AuthServiceRegistrar
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddTransient<AuthService>();

        return services;
    }
}