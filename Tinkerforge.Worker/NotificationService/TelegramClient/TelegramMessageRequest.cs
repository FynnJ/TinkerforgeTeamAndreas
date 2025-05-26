namespace Tinkerforge.Worker.NotificationService.TelegramClient;

public record TelegramMessageRequest
{
    // ReSharper disable once InconsistentNaming
    public required string chat_id { get; set; }
    // ReSharper disable once InconsistentNaming
    public required string text { get; set; }
}