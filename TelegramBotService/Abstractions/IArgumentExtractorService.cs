using Telegram.Bot.Types;

namespace TelegramBotService.Abstractions;

// review - Service is a redundant word
public interface IArgumentExtractorService
{
    ICommandArgs GetArgs(Update update);
}
