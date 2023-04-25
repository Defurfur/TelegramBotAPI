using ReaSchedule.Models;
using Telegram.Bot.Types;

namespace TelegramBotService.Abstractions;

public interface IMessageSender
{
    Task<Message> ShowStartMessage(Message message);
    Task<Message> GroupFoundMessage(Message message);
    Task<Message> InvalidGroupInputMessage(Message message);
    Task<Message> GroupNotFoundMessage(Message message);
    Task<Message> SendDefaultSubscriptionSettings(Message message);
    Task<Message> SendChangeGroupInfo(Message message, string groupName);
    Task<Message> ChangeGroupSuccess(Message message);
    Task<Message> SendMessageWithSomeText(Message message, string text);
    Task<Message> SomethingWentWrongMessage(Message message);
    Task<Message> SendGroupSearchInProcess(Message message);
    Task<Message> DownloadScheduleMessageWithKeyboard(Message message, string text);
    Task<Message> SendMessageWithSomeText(long chatId, string text);
    Task<Message> SendSubscriptionSettings(SubscriptionSettings settings, Message message, bool subscriptionEnabled);
    Task<Message> SendDefaultKeyboard(Message message);
}
