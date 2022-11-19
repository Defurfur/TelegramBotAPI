using Telegram.Bot.Types;
using TelegramBot.Abstractions;

namespace TelegramBot.Commands;

public class ShowGroupFoundMessageCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _reciever;
    private readonly Message _message;
    public ShowGroupFoundMessageCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _reciever = args.MessageSender!;
    }

    public async Task<Message> ExecuteAsync()
    {
        return await _reciever.GroupFoundMessage(_message);
    }
}
