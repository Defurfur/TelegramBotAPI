using Telegram.Bot.Types;

namespace TelegramBot.Abstractions;

public interface IArgumentExtractorService
{
    ICommandArgs GetArgs(Update update);
}
