using Telegram.Bot.Types;

namespace TelegramBotService.Abstractions;

public interface IMessageSender
{
    Task<Message> ShowStartMessage(Message message);
    Task<Message> GroupFoundMessage(Message message);
    Task<Message> InvalidGroupInputMessage(Message message);
    Task<Message> GroupNotFoundMessage(Message message);
    Task<Message> SendSubscriptionSettings(Message message);
    Task<Message> SendChangeGroupInfo(Message message);
    Task<Message> ChangeGroupSuccess(Message message);
    Task<Message> SendMessageWithSomeText(Message message, string text);
}
