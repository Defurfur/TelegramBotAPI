using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using TelegramBot.Services;
using User = ReaSchedule.Models.User;

namespace TelegramBot.Interfaces
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

    }


}
