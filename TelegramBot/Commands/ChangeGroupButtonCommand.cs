using Telegram.Bot.Types;
using TelegramBot.Abstractions;
using TelegramBot.Services;

namespace TelegramBot.Commands;

public class ChangeGroupButtonCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly Message _message;
    public ChangeGroupButtonCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
    }

    public async Task<Message> ExecuteAsync()
    {
        return await _sender.SendChangeGroupInfo(_message);
    }
}
