using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Services;
using TelegramBotService.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotService.Services;

public interface IGroupSearchPipeline
{
    Task<GroupHasBeenFound> Execute(Message message, User? user = null);
}

public class GroupSearchPipeline : IGroupSearchPipeline
{
    private readonly IBrowserWrapper _browserWrapper;
    private readonly ScheduleDbContext _context;
    private Message? _message;
    private readonly IParserPipeline _parserPipeline;

    public GroupSearchPipeline(
        ScheduleDbContext context,
        IBrowserWrapper browserWrapper,
        IParserPipeline parserPipeline)
    {
        _context = context;
        _browserWrapper = browserWrapper;
        _parserPipeline = parserPipeline;
    }


    public async Task<GroupHasBeenFound> Execute(Message message, User? user = null)
    {
        if (message == null)
            ArgumentNullException.ThrowIfNull(message);

        _message = message;

        if(TryFindGroupInDb(_message.Text!, out var group))
        {
            var task = user is null ? TryRegisterUserAsync(group!) : TryChangeUsersGroupAsync(user, group!);   

            await task;
            return GroupHasBeenFound.InDatabase;
        }

        bool groupFoundInSchedule = await TryFindGroupInScheduleAndUpdateContext(_message);

        if (groupFoundInSchedule)
        {
            TryFindGroupInDb(_message.Text!, out group);

            var task = user is null ? TryRegisterUserAsync(group!) : TryChangeUsersGroupAsync(user, group!);

            await task;
            return GroupHasBeenFound.InSchedule;
        }

        return GroupHasBeenFound.False;
    }

    private bool TryFindGroupInDb(string text, out ReaGroup? reaGroup)
    {
        text = text.Replace("/change ", "");

        var group = _context
            .ReaGroups
            .FirstOrDefault(x =>
            x!.GroupName == text.ToLower().Trim());

        reaGroup = group;
        return reaGroup != null;
    }

    private async Task TryRegisterUserAsync(ReaGroup group)
    {
        var newUser = new ReaSchedule.Models.User()
        {
            ChatId = _message!.Chat.Id,
            ReaGroup = group,
            ReaGroupId = group.Id,
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

    } 
    private async Task TryChangeUsersGroupAsync(User user, ReaGroup reaGroup)
    {
        user.ReaGroup = reaGroup;
        user.ReaGroupId = reaGroup.Id;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

    }

    private async Task<bool> TryFindGroupInScheduleAndUpdateContext(Message message)
    {
        var groupAsString = message.Text!.Replace("/change ", "");

        var url = "https://rasp.rea.ru/?q=" + groupAsString.Replace("/", "%2F");

        if (!_browserWrapper.IsInit)
            await _browserWrapper.Init();

        var page = await _browserWrapper.Browser!.NewPageAsync();

        await page.GoToAsync(url);
        await page.WaitForNavigationAsync(new PuppeteerSharp.NavigationOptions { Timeout = 0});

        var jToken = await page.EvaluateExpressionAsync(JsScriptLibrary.CheckForGroupExistance(groupAsString));

        var exists = Convert.ToBoolean(jToken.ToString());
        if (exists)
            await AddNewGroupAsync(groupAsString);

        return exists;

    }

    private async Task AddNewGroupAsync(string groupName)
    {
        if (_context.ReaGroups.Any(x => x.GroupName == groupName))
            return;

        
        _context.Add(new ReaGroup(){ GroupName = groupName });

        await _context.SaveChangesAsync();

        var createdGroup = _context
            .ReaGroups
            .First(x => x.GroupName == groupName);

        var updatedReaGroup = await _parserPipeline.ParseAndUpdate(createdGroup);

        createdGroup.ScheduleWeeks = updatedReaGroup.ScheduleWeeks;
        createdGroup.Hash = updatedReaGroup.Hash;

        await _context.SaveChangesAsync();
    }

}
