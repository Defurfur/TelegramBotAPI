using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;

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
        var groupHasBeenFound = await _groupSearchPipeline.Execute(_message, _user);

        var result = groupHasBeenFound switch
        {
            GroupHasBeenFound.InDatabase => _sender.ChangeGroupSuccess(_message),
            GroupHasBeenFound.InSchedule => _sender.ChangeGroupSuccess(_message),
            _ => _sender.GroupNotFoundMessage(_message)
        };
        return await result;
    }
}
