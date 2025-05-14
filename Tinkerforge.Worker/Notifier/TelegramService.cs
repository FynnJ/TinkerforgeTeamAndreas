using Tinkerforge.Worker.Notifier.TelegramClient;

namespace Tinkerforge.Worker.Notifier;

public class TelegramService(
    ILogger<TelegramService> logger,
    ITelegramClient telegramClient,
    TelegramConfiguration telegramConfiguration)
    : ITelegramService
{
    public async Task SendMessageAsync(string message)
    {
        try
        {
            var response = await telegramClient.SendMessageAsync(telegramConfiguration.Token!,
                new TelegramMessageRequest
                {
                    chat_id = telegramConfiguration.ChatId!,
                    text = message
                });

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Message sent successfully");
            }
            else
            {
                logger.LogError("Error sending message. Error: {Error}", response.StatusCode);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}