using RestEase;

namespace Tinkerforge.Worker.Notifier.TelegramClient;

public interface ITelegramClient
{
    [Post("bot{botToken}/sendMessage")]
    Task<HttpResponseMessage> SendMessageAsync(
        [Path] string botToken,
        [Body] TelegramMessageRequest request);
}