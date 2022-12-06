﻿namespace TelegramBotService;

public record BotConfiguration
{
    public string? BotToken { get; init; }

    // Open API is unable to process urls with ":" symbol
    public string? EscapedBotToken => BotToken?.Replace(':', '_');

    public string? HostAddress { get; init; }
}