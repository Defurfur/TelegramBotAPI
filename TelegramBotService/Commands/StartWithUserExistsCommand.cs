using Telegram.Bot.Types;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Commands;

public class StartWithUserExistsCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly Message _message;
    public StartWithUserExistsCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
    }

    public async Task<Message> ExecuteAsync()
    {
        return await _sender.SendDefaultKeyboard(_message);
    }
}




