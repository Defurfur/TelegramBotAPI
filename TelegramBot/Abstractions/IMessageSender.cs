using Jint.Native;
using ReaSchedule.Models;
using Telegram.Bot.Types;

namespace TelegramBot.Abstractions;

public interface IMessageSender
{
    Task<Message> ShowStartMessage(Message message);
    Task<Message> GroupFoundMessage(Message message);
    Task<Message> InvalidGroupInputMessage(Message message);
    Task<Message> GroupNotFoundMessage(Message message);
    Task<Message> SendSubscriptionSettings(Message message);
    Task<Message> SendChangeGroupInfo(Message message);
    Task<Message> ChangeGroupSuccess(Message message);
}
