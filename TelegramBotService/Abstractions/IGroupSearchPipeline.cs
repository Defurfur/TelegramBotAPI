using ReaSchedule.Models;
using TelegramBotService.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotService.Abstractions;

public interface IGroupSearchPipeline
{
    Task<GroupSearchState> RegisterUserAsync(Message message);
    Task<GroupSearchState> ChangeUserGroupAsync(Message message, User user);
}
