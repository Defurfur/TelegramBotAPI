using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Services;
using TelegramBotService.Abstractions;
using TelegramBotService.BackgroundTasks;
using TelegramBotService.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotService.Services;

public record MessageAndUser
{

    public required Message Message { get; set; }
    public required User User { get; set; }
}

public class GroupSearchPipeline : IGroupSearchPipeline
{
    private readonly IQueue _queue;
    private readonly IBrowserWrapper _browserWrapper;
    private readonly ScheduleDbContext _context;
    private Message? _message;
    private readonly IContextUpdateService _contextUpdateService;
    private readonly IParserPipeline _parserPipeline;

    public GroupSearchPipeline(
        ScheduleDbContext context,
        IBrowserWrapper browserWrapper,
        IParserPipeline parserPipeline,
        IQueue queue,
        IContextUpdateService contextUpdateService)
    {
        _context = context;
        _browserWrapper = browserWrapper;
        _parserPipeline = parserPipeline;
        _queue = queue;
        _contextUpdateService = contextUpdateService;
    }
    /// <summary>
    /// Method used to register user when he first starts a bot. If a group from <paramref name="message"/>.Text hasn't been 
    /// found in database, adds a background task for its search on the website
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/>is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="message.Text"/>is empty.</exception>
    public async Task<GroupSearchState> ExecuteAsync(Message message)
    {
        
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        ArgumentException.ThrowIfNullOrEmpty(message.Text, nameof(message.Text));

        message.Text = message.Text.Trim().ToLower();

        if(_contextUpdateService.TryFindGroupInDb(message.Text, out var group))
        {
            await _contextUpdateService.TryRegisterUserAsync(group!, message.Chat.Id);
            return GroupSearchState.FoundInDatabase;
        }

        _queue.QueueInvocableWithPayload<TryFindGroupAndRegisterUserInvocable, Message>(message);

        return GroupSearchState.InProcess;
    }
    /// <summary>
    /// Method used to change user's group. If a group <paramref name="message"/>.Text hasn't been 
    /// found in database, adds a background task 
    /// for its search on the website
    /// </summary>
    /// <param name="message"></param>
    /// <param name="user"></param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/>is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="user"/>is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="message.Text"/>is empty.</exception>
    public async Task<GroupSearchState> ExecuteAsync(Message message, User user)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentException.ThrowIfNullOrEmpty(message.Text, nameof(message.Text));

        message.Text = message.Text.Trim().ToLower();

        if (_contextUpdateService.TryFindGroupInDb(message.Text, out var group))
        {
            await _contextUpdateService.TryChangeUsersGroupAsync(user, group!);
            return GroupSearchState.FoundInDatabase;
        }

        var MessageAndUser = new MessageAndUser { Message = message, User = user };


        _queue.QueueInvocableWithPayload<TryFindGroupAndChangeUserInvocable, MessageAndUser>(MessageAndUser);

        return GroupSearchState.InProcess;
    }

    //private async Task AddNewGroupAsync(string groupName)
    //{
    //    if (_context.ReaGroups.Any(x => x.GroupName == groupName))
    //        return;

        
    //    _context.Add(new ReaGroup(){ GroupName = groupName });

    //    await _context.SaveChangesAsync();

    //    var createdGroup = _context
    //        .ReaGroups
    //        .First(x => x.GroupName == groupName);

    //    var updatedReaGroup = await _parserPipeline.ParseAndUpdate(createdGroup);

    //    createdGroup.ScheduleWeeks = updatedReaGroup.ScheduleWeeks;
    //    createdGroup.Hash = updatedReaGroup.Hash;

    //    await _context.SaveChangesAsync();
    //}

}
