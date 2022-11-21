using Telegram.Bot.Types;

namespace TelegramBotService.Abstractions;

public interface IArgumentExtractorService
{
    ICommandArgs GetArgs(Update update);
}
