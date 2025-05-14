namespace Tinkerforge.Worker.NotificationService;

public record TelegramConfiguration
{
    public string? BaseUrl { get; set; }
    public string? Token { get; set; }
    public string? ChatId { get; set; }
}