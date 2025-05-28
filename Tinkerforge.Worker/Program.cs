using Tinkerforge.Worker;
using Tinkerforge.Worker.DataMonitoring;
using Tinkerforge.Worker.MotionService;
using Tinkerforge.Worker.NotificationService;
using Tinkerforge.Worker.Authorization;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices();
builder.Services.AddNotificationServices(builder.Configuration);
builder.Services.AddMotionServices();
builder.Services.AddDataMonitoringServices();
builder.Services.AddAuthServices();

var host = builder.Build();
await host.RunAsync();