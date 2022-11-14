using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleWorker.Services;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Services;

public interface IGroupSearchPipeline
{
    Task<GroupHasBeenFound> Execute(Message message);
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

    public async Task<GroupHasBeenFound> Execute(Message message)
    {
        if (message == null)
            ArgumentNullException.ThrowIfNull(message);

        _message = message;

        if(TryFindGroupInDb(out var group))
        {
            await TryRegisterUser(group);
            return GroupHasBeenFound.InDatabase;
        }

        var groupFoundInSchedule = await TryFindGroupInSchedule();

        if (groupFoundInSchedule)
        {
            // Здесь надо будет воткнуть TryRegisterUser 
            return GroupHasBeenFound.InSchedule;
        }

        return GroupHasBeenFound.False;
    }

    private bool TryFindGroupInDb(out ReaGroup? reaGroup)
    {
        var group = _context
            .ReaGroups
            .FirstOrDefault(x =>
            x!.GroupName == _message!.Text!.ToLower().Trim());

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
