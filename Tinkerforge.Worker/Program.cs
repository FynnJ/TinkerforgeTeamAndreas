using Tinkerforge.Worker;
using Tinkerforge.Worker.DataMonitoring;
using Tinkerforge.Worker.MotionService;
using Tinkerforge.Worker.NotificationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices();
builder.Services.AddNotificationServices(builder.Configuration);
builder.Services.AddMotionServices();
builder.Services.AddDataMonitoringServices();

var host = builder.Build();
await host.RunAsync();