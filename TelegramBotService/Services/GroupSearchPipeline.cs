using ReaSchedule.DAL;
using ReaSchedule.Models;
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
            var task = user is null ? TryRegisterUser(group!) : TryChangeUsersGroup(user, group!);   

            await task;
            return GroupHasBeenFound.InDatabase;
        }

        var groupFoundInSchedule = await TryFindGroupInScheduleAndUpdateContext();

        if (groupFoundInSchedule)
        {
            var task = user is null ? TryRegisterUser(group!) : TryChangeUsersGroup(user, group!);
            await task;
            return GroupHasBeenFound.InSchedule;
        }

        return GroupHasBeenFound.False;
    }

    private bool TryFindGroupInDb(string text, out ReaGroup? reaGroup)
    {
        var group = _context
            .ReaGroups
            .FirstOrDefault(x =>
            x!.GroupName == text.ToLower().Trim());

        reaGroup = group;
        return reaGroup != null;
    }

    private async Task TryRegisterUser(ReaGroup group)
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
    private async Task TryChangeUsersGroup(User user, ReaGroup reaGroup)
    {
        user.ReaGroup = reaGroup;
        user.ReaGroupId = reaGroup.Id;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

    }

    private async Task<bool> TryFindGroupInScheduleAndUpdateContext()
    {
        var url = "https://rasp.rea.ru/?q=" + _message!.Text!.Replace("/", "%2F");
        if (!_browserWrapper.IsInit)
            await _browserWrapper.Init();

        var page = await _browserWrapper.Browser!.NewPageAsync();
        await page.GoToAsync(url);
        await page.WaitForNavigationAsync();

        var jToken = await page.EvaluateExpressionAsync(JsScriptLibrary.CheckForGroupExistance(_message.Text));
        var exists = Convert.ToBoolean(jToken.ToString());
        if (exists)
            await AddNewGroup(_message.Text);

        return exists;

    }

    private async Task AddNewGroup(string groupName)
    {
        var newReaGroup = new ReaGroup() { GroupName = groupName};
        var updatedReaGroup = await _parserPipeline.ParseAndUpdate(newReaGroup);

        await _context.AddAsync(updatedReaGroup);
        await _context.SaveChangesAsync();
    }

}
