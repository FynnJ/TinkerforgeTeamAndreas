using RestEase;
using Tinkerforge;
using Tinkerforge.Worker;
using Tinkerforge.Worker.MotionSensor;
using Tinkerforge.Worker.Notifier;
using Tinkerforge.Worker.Notifier.TelegramClient;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<NotifierWorker>();
builder.Services.AddHostedService<MotionWorker>();

builder.Services.AddSingleton<IPConnection>(_ =>
{
    var ipConnection = new IPConnection();
    ipConnection.Connect("172.20.10.242", 4223);
    return ipConnection;
});

builder.Services.AddSingleton<TelegramConfiguration>(_ =>
    builder.Configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>()
    ?? new TelegramConfiguration());

builder.Services.AddTransient<ITelegramClient>(serviceProvider =>
    RestClient.For<ITelegramClient>(serviceProvider.GetRequiredService<TelegramConfiguration>().BaseUrl));

builder.Services.AddTransient<ITelegramService, TelegramService>();

builder.Services.AddTransient<NotifierService>();
builder.Services.AddTransient<MotionService>();

var host = builder.Build();
await host.RunAsync();