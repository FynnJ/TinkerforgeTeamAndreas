using Tinkerforge.Worker;
using Tinkerforge.Worker.MotionService;
using Tinkerforge.Worker.NotificationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServices();
builder.Services.AddNotificationServices(builder.Configuration);
builder.Services.AddMotionServices();

var host = builder.Build();
await host.RunAsync();