using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotService.Models;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Abstractions
{
    public interface ICommandArgs
    {
        User? User { get; set; }
        Update Update { get; set; }
        CallbackQuery? Callback { get; set; }
        UpdateType UpdateType { get; set; }
        OperationType OperationType { get; set; }
        IMessageSender? MessageSender { get; set; }
        IGroupSearchPipeline? GroupSearchPipeline { get; set; }
        CallbackType CallbackType { get; set; }
        ICallbackMessageUpdater? CallbackMessageUpdater { get; set; }
        IUserUpdater? UserUpdater { get; set; }
        IScheduleLoader? ScheduleLoader { get; set; }
        IContextUpdateService? ContextUpdateService { get; set; }
    }


}
