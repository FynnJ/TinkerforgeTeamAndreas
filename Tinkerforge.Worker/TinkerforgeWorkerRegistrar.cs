namespace Tinkerforge.Worker;

public static class TinkerforgeWorkerRegistrar
{
    public static IServiceCollection AddTinkerforgeWorker(this IServiceCollection services)
    {
        var workers = typeof(IWorkerAssemblyMarker).Assembly
            .GetTypes()
            .Where(type =>
                type.IsAssignableTo(typeof(TinkerforgeWorkerBase))
                && type.IsClass
                && !type.IsAbstract);

        foreach (var worker in workers)
        {
            // services.AddTransient(typeof(IHostedService), serviceProvider =>
            //     (IHostedService)ActivatorUtilities.CreateInstance(serviceProvider, workerType));

            services.AddSingleton<IHostedService>(_ => 
                (IHostedService)Activator.CreateInstance(worker)!);
        }

        return services;
    }

    // public static void RegisterAllImplementationsOfIX(this IServiceCollection services)
    // {
    //     var interfaceType = typeof(IWorkerAssemblyMarker);
    //     var assembly = Assembly.GetExecutingAssembly();
    //
    //     var implementations = assembly.GetTypes()
    //         .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
    //
    //     foreach (var impl in implementations)
    //     {
    //         services.AddTransient(interfaceType, impl);
    //     }
    // }
}