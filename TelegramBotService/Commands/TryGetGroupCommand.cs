using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;

namespace TelegramBotService.Commands;

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
            _ => _sender.GroupNotFoundMessage(_message)
        };
        return await result;
    }
}
