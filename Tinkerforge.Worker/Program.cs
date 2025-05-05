using Tinkerforge;
using Tinkerforge.Worker;
using Tinkerforge.Worker.DataMonitoring;
using Tinkerforge.Worker.Notifier;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IPConnection>(_ =>
{
    var ipConnection = new IPConnection();
    ipConnection.Connect("172.20.10.242", 4223);
    return ipConnection;
});

builder.Services.AddTransient<NotifierService>();
builder.Services.AddTransient<DataMonitoringService>();

var host = builder.Build();
await host.RunAsync();