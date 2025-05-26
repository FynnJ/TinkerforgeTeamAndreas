namespace Tinkerforge.Worker.NotificationService;

public interface ITelegramService
{
    Task SendMessageAsync(string message);
}