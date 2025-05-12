namespace Tinkerforge.Worker.Notifier;

public interface ITelegramService
{
    Task SendMessageAsync(string message);
}