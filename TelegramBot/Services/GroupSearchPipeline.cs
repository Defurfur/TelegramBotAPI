using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleWorker.Services;
using TelegramBot.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Services;

public interface IGroupSearchPipeline
{
    Task<GroupHasBeenFound> Execute(Message message, User? user = null);
}

public class GroupSearchPipeline : IGroupSearchPipeline
{
    private readonly IBrowserWrapper _browserWrapper;
    private readonly ScheduleDbContext _context;
    private Message? _message;

    public GroupSearchPipeline(
        ScheduleDbContext context,
        IBrowserWrapper browserWrapper)
    {
        _context = context;
        _browserWrapper = browserWrapper;
    }
  

    public async Task<GroupHasBeenFound> Execute(Message message, User? user = null)
    {
        if (message == null)
            ArgumentNullException.ThrowIfNull(message);

        _message = message;

        if(TryFindGroupInDb(_message.Text, out var group))
        {
            var task = user is null ? TryRegisterUser(group) : TryChangeUsersGroup(user, group);   

            await task;
            return GroupHasBeenFound.InDatabase;
        }

        var groupFoundInSchedule = await TryFindGroupInSchedule();

        if (groupFoundInSchedule)
        {
            // Здесь надо будет воткнуть onSearchSuccessOperation 
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

    private async Task<bool> TryFindGroupInSchedule()
    {
        var url = "https://rasp.rea.ru/?q=" + _message!.Text!.Replace("/", "%2F");
        if (!_browserWrapper.IsInit)
            await _browserWrapper.Init();

        var page = await _browserWrapper.Browser!.NewPageAsync();
        await page.GoToAsync(url);
        await page.WaitForNavigationAsync();

        var jToken = await page.EvaluateExpressionAsync(JsScriptLibrary.CheckForGroupExistance(_message.Text));
        var exists = Convert.ToBoolean(jToken.ToString());
        // Здесь надо будет как-то попинать воркера чтобы он обновил расписание в базе
        return exists;

    }

}
