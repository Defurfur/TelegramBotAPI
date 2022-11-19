using Jint.Parser.Ast;
using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Abstractions;
using TelegramBot.Models;
using User = ReaSchedule.Models.User;

namespace TelegramBot.Services;

public class ArgumentExtractorService : IArgumentExtractorService
{
    private readonly ScheduleDbContext _context;
    private readonly IMessageSender _messageSender;
    private readonly IGroupSearchPipeline _groupSearchPipeline;
    private readonly ICallbackMessageUpdater _callbackMessageUpdater;
    private readonly IUserUpdater _userUpdater;

    public ArgumentExtractorService(
        ScheduleDbContext context,
        IMessageSender messageSender,
        IGroupSearchPipeline groupSearchPipeline,
        ICallbackMessageUpdater callbackMessageUpdater,
        IUserUpdater userUpdater)
    {
        _context = context;
        _messageSender = messageSender;
        _groupSearchPipeline = groupSearchPipeline;
        _callbackMessageUpdater = callbackMessageUpdater;
        _userUpdater = userUpdater;
    }

    public ICommandArgs GetArgs(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);

        ICommandArgs commandArgs = new CommandArgs() { Update = update };

        commandArgs.User = null;

        if(update.Message is not null || update.CallbackQuery is not null)
        {
            var chatId = update.Message == null ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
            commandArgs.User = TryGetUser(chatId);
        }

        commandArgs.MessageSender = _messageSender;

        commandArgs.Callback = update.CallbackQuery;

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
            && CheckMessageForGroupInput(update.Message.Text))
        {
            commandArgs.OperationType = OperationType.IsGroupInput;
        }

        if(commandArgs.User != null && update.Message != null)
        {
            commandArgs.OperationType = update.Message.Text switch
            {
                "Загрузить расписание" => OperationType.DownloadScheduleRequest,
                "Смена группы" => OperationType.ChangeGroupButtonPressed,
                "Настройки подписки" => OperationType.ChangeSubscriptionSettingsRequest,
                _ => OperationType.Other
            };
        }
        if(commandArgs.User != null 
            && update.Message != null 
            && update.Message.Text.Split(' ')[0] == "/change"
            && CheckMessageForGroupInput(update.Message.Text.Split(' ')[1]))
        {
            commandArgs.OperationType = OperationType.GroupChangeCommand;
        }

        commandArgs = ProcessCallback(commandArgs, update);




        return commandArgs;
    }
    private User? TryGetUser(long id)
    {
        var user = _context
            .Users
            .FirstOrDefault(x => x.ChatId == id);
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
    private bool CheckMessageForGroupInput(string text)
    {
        var cleanedText = text.ToLower().Trim();

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
    private ICommandArgs ProcessCallback(ICommandArgs args, Update update)
    {
        args.Callback = update.CallbackQuery;
        if (args.Callback is null)
            return args;

        args.CallbackMessageUpdater = _callbackMessageUpdater;
        args.UserUpdater = _userUpdater;

        return args;


    }

}