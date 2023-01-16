namespace TelegramBotAPI;

public record BotConfiguration
{
    public string? BotToken { get; init; }

    public string? EscapedBotToken => BotToken?.Replace(':', '_');

    public string? HostAddress { get; init; }
}