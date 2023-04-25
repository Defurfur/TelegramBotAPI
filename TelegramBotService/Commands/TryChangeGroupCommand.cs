using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.Commands;

public class TryChangeGroupCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly ReaSchedule.Models.User _user;
    private readonly Message _message;
    private readonly IGroupSearchPipeline _groupSearchPipeline;
    public TryChangeGroupCommand(ICommandArgs args)
    {
        _user = args.User!;
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
        _groupSearchPipeline = args.GroupSearchPipeline!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var groupHasBeenFound = await _groupSearchPipeline.ChangeUserGroupAsync(_message, _user);

        var result = groupHasBeenFound switch
        {
            GroupSearchState.FoundInDatabase => _sender.ChangeGroupSuccess(_message),
            GroupSearchState.InProcess => _sender.SendGroupSearchInProcess(_message),
            _ => _sender.GroupNotFoundMessage(_message)
        };
        return await result;
    }
}
