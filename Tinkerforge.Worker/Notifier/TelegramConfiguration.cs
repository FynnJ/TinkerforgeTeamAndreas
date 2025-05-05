namespace Tinkerforge.Worker.Notifier;

public record TelegramConfiguration
{
    public string? BaseUrl { get; set; }
    public string? Token { get; set; }
    public string? ChatId { get; set; }
}