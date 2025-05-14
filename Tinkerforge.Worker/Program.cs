using Tinkerforge;
using Tinkerforge.Worker;
using Tinkerforge.Worker.MotionSensor;
using Tinkerforge.Worker.NotificationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<MotionWorker>();

builder.Services.AddSingleton<IPConnection>(_ =>
{
    var ipConnection = new IPConnection();
    ipConnection.Connect("172.20.10.242", 4223);
    return ipConnection;
});

builder.Services.AddNotificationServices(builder.Configuration);
builder.Services.AddTransient<MotionService>();

var host = builder.Build();
await host.RunAsync();