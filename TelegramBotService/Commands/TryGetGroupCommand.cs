using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

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
        var groupHasBeenFound = await _groupSearchPipeline.ExecuteAsync(_message);

        var result = groupHasBeenFound switch
        {
            GroupSearchState.FoundInDatabase => _sender.GroupFoundMessage(_message),
            GroupSearchState.InProcess => _sender.SendGroupSearchInProcess(_message),
            _ => _sender.SomethingWentWrongMessage(_message),
        };
        return await result;
    }
}
