using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Interfaces;
using TelegramBot.Services;

namespace TelegramBot.Models
{
    public class CommandArgs : ICommandArgs
    {
        public ReaSchedule.Models.User? User { get; set; } = null;
        public Update Update { get; set; }
        public CallbackQuery? Callback { get; set; }
        public UpdateType UpdateType { get; set; }
        public OperationType OperationType { get; set; } = OperationType.Other;
        public IMessageSender? MessageSender { get; set; }
        public IGroupSearchPipeline? GroupSearchPipeline { get; set; }
    }
    public enum OperationType
    {
        NeedsSearchInDb,
        NeedsSearchInSchedule,
        IsStartCommand,
        IsGroupInput,
        Other,
    }
}
