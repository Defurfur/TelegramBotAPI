using Telegram.Bot.Types;
using TelegramBot.Interfaces;
using TelegramBot.Services;

namespace TelegramBot.Commands;

public class TryGetGroupCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly Message _message;
    private readonly IGroupSearchPipeline _groupSearchPipeline;
    public TryGetGroupCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
        _groupSearchPipeline = args.GroupSearchPipeline!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var groupHasBeenFound = await _groupSearchPipeline.Execute(_message);

        var result = groupHasBeenFound switch
        {
            GroupHasBeenFound.InDatabase => _sender.GroupFoundMessage(_message),
            GroupHasBeenFound.InSchedule => _sender.GroupFoundMessage(_message),
            GroupHasBeenFound.False => _sender.GroupNotFoundMessage(_message),
            _ => _sender.InvalidGroupInputMessage(_message),
        };
        return await result;
    }
}
