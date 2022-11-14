using Jint.Parser.Ast;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Interfaces;
using TelegramBot.Models;
using User = ReaSchedule.Models.User;

namespace TelegramBot.Services;

public interface IArgumentExtractorService
{
    ICommandArgs GetArgs(Update update);
}

public class ArgumentExtractorService : IArgumentExtractorService
{
    private readonly ScheduleDbContext _context;
    private readonly IMessageSender _messageSender;
    private readonly IGroupSearchPipeline _groupSearchPipeline;

    public ArgumentExtractorService(
        ScheduleDbContext context,
        IMessageSender messageSender,
        IGroupSearchPipeline groupSearchPipeline)
    {
        _context = context;
        _messageSender = messageSender;
        _groupSearchPipeline = groupSearchPipeline;
    }

    public ICommandArgs GetArgs(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);

        ICommandArgs commandArgs = new CommandArgs() { Update = update };

        commandArgs.User = update.Message == null ? null : TryGetUser(update.Message);

        commandArgs.MessageSender = _messageSender;

        commandArgs.Callback = update.CallbackQuery ?? null;

        commandArgs.UpdateType = update.Type;

        commandArgs.GroupSearchPipeline = _groupSearchPipeline;


        if (commandArgs.User == null
            && commandArgs.UpdateType == UpdateType.Message
            && TryGetStartCommand(update))
        {
            commandArgs.OperationType = OperationType.IsStartCommand;
        }


        if (commandArgs.User == null
            && commandArgs.UpdateType == UpdateType.Message
            && update.Message!.Text != null
            && CheckMessageForGroupInput(update))
        {
            commandArgs.OperationType = OperationType.IsGroupInput;
        }


        return commandArgs;
    }
    private User? TryGetUser(Message? message)
    {
        if (message is null)
            return null;
        var user = _context
            .Users
            .FirstOrDefault(x => x.ChatId == message.Chat.Id);
        if (user == default)
            return null;
        return user;
    }
    private bool TryGetStartCommand(Update update)
    {
        if (update.Message!.Text == null)
            return false;
        return update.Message.Text.Split(' ')[0] == "/start";
    }
    private bool CheckMessageForGroupInput(Update update)
    {
        var message = update.Message;
        var cleanedText = message!.Text!.ToLower().Trim();

        bool firstCondition =
            !cleanedText.Contains(' ')
            && cleanedText.Contains('.')
            && cleanedText.Contains('/')
            && cleanedText.Contains('-')
            && cleanedText.Length >= 13
            && cleanedText.Length <= 16;

        bool secondCondition = cleanedText.StartsWith("97в/") || cleanedText.StartsWith("97з/");


        return firstCondition || secondCondition;
    }

}