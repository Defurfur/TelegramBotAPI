using ReaSchedule.Models;
using TelegramBotService.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotService.Abstractions;

public interface IGroupSearchPipeline
{
    Task<GroupSearchState> ExecuteAsync(Message message);
    Task<GroupSearchState> ExecuteAsync(Message message, User user);
}
