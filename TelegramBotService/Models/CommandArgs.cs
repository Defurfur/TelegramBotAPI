using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Models;

public class CommandArgs : ICommandArgs
{
    public ReaSchedule.Models.User? User { get; set; } = null;
    public Update Update { get; set; }
    public CallbackQuery? Callback { get; set; }
    public UpdateType UpdateType { get; set; }
    public OperationType OperationType { get; set; } = OperationType.Other;
    public CallbackType CallbackType { get; set; } = CallbackType.None;
    public IUserUpdater? UserUpdater { get; set; }
    public ICallbackMessageUpdater? CallbackMessageUpdater{ get; set; }
    public IMessageSender? MessageSender { get; set; }
    public IGroupSearchPipeline? GroupSearchPipeline { get; set; }
    public IScheduleLoader? ScheduleLoader { get; set; } 
    public IContextUpdateService? ContextUpdateService { get; set; }
}
