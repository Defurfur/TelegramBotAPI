using Telegram.Bot.Types;
using TelegramBot.Interfaces;

namespace TelegramBot.Commands;

public class InvalidGroupInputCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _reciever;
    private readonly Message _message;
    public InvalidGroupInputCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _reciever = args.MessageSender!;
    }

    public async Task<Message> ExecuteAsync()
    {
        return await _reciever.InvalidGroupInputMessage(_message);
    }
}