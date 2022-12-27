using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Update = Telegram.Bot.Types.Update;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Services;

// review - overall complexity is overwhelming. Some refactoring needed. 
public class ArgumentExtractorService : IArgumentExtractorService
{
    private readonly ScheduleDbContext _context;
    private readonly IMessageSender _messageSender;
    private readonly IGroupSearchPipeline _groupSearchPipeline;
    private readonly ICallbackMessageUpdater _callbackMessageUpdater;
    private readonly IUserUpdater _userUpdater;
    private readonly IScheduleLoader _scheduleLoader;
    private readonly IContextUpdateService _contextUpdateService;

    public ArgumentExtractorService(
        ScheduleDbContext context,
        IMessageSender messageSender,
        IGroupSearchPipeline groupSearchPipeline,
        ICallbackMessageUpdater callbackMessageUpdater,
        IUserUpdater userUpdater,
        IScheduleLoader scheduleLoader,
        IContextUpdateService contextUpdateService)
    {
        _context = context;
        _messageSender = messageSender;
        _groupSearchPipeline = groupSearchPipeline;
        _callbackMessageUpdater = callbackMessageUpdater;
        _userUpdater = userUpdater;
        _scheduleLoader = scheduleLoader;
        _contextUpdateService = contextUpdateService;
    }

    public ICommandArgs GetArgs(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);

        ICommandArgs args = new CommandArgs() {
            Update = update,
            UpdateType = update.Type,
            Callback = update.CallbackQuery,
            GroupSearchPipeline = _groupSearchPipeline,
            MessageSender = _messageSender,

        };


        TryGetUser(args);

        ProcessMessageText(args);

        ProcessCallback(args);

        return args;
    }

    // review - suggestion - Method with word 'Try' should return boolean value and have out param. 
    // review - suggestion - Maybe use async overload? 
    private ICommandArgs TryGetUser(ICommandArgs args)
    {
        if (args.Update.Message is null && args.Update.CallbackQuery is null)
            return args;

        var chatId = args.Update.Message == null
            ? args.Update.CallbackQuery!.Message!.Chat.Id
            : args.Update.Message.Chat.Id;

        var user = _context
            .Users
            .Include(x => x.SubscriptionSettings)
            .FirstOrDefault(x => x.ChatId == chatId);

        args.User = user;

        return args;

    }

    // review - warning - avoid using inline local functions
    private ICommandArgs ProcessMessageText(ICommandArgs args)
    {
        bool condition = 
            args.Update.Type == UpdateType.Message
            && args.Update.Message != null
            && args.Update.Message.Text != null;

        if (!condition)
            return args;

        var text = args.Update.Message!.Text!;

        args = TryGetBugCommand(args);

        args = ProcessExistingUser(args);

        args = ProcessNotExistingUser(args);

        return args;

        ICommandArgs TryGetBugCommand(ICommandArgs args)
        {
            bool innerCondition = text.Contains("/bug");

            if (!innerCondition)
                return args;

            args.OperationType = OperationType.BugCommand;
            args.ContextUpdateService = _contextUpdateService;

            return args;
        }

        ICommandArgs ProcessExistingUser(ICommandArgs args)
        {
            bool innerCondition = args.User != null;

            if (!innerCondition) 
                return args;

            args = TryGetKeyboardCommand(args);
            args = TryGetChangeGroupCommand(args);

            return args;



            ICommandArgs TryGetKeyboardCommand(ICommandArgs args)
            {
                args.OperationType = args.Update!.Message!.Text switch
                {
                    "Загрузить расписание" => OperationType.DownloadScheduleRequest,
                    "Смена группы" => OperationType.ChangeGroupButtonPressed,
                    "Настройки подписки" => OperationType.ChangeSubscriptionSettingsRequest,
                    _ => args.OperationType
                };

                if (args.OperationType == OperationType.DownloadScheduleRequest)
                    args.ScheduleLoader = _scheduleLoader;

                if (args.OperationType == OperationType.ChangeGroupButtonPressed)
                    args.UserUpdater = _userUpdater;

                return args;
            }

            ICommandArgs TryGetChangeGroupCommand(ICommandArgs args)
            {
                var text = args.Update.Message!.Text!;
                var textSplit = text.Split(" ");

                if (textSplit.Length != 2 || textSplit[0] != "/change")
                    return args;

                var groupAsString = textSplit[1];

                bool firstCondition =
                !groupAsString.Contains(' ')
                && groupAsString.Contains('.')
                && groupAsString.Contains('/')
                && groupAsString.Contains('-')
                && groupAsString.Length >= 13
                && groupAsString.Length <= 19;

                bool secondCondition = groupAsString.StartsWith("97в/") || groupAsString.StartsWith("97з/");

                if(firstCondition || secondCondition)
                {
                    args.OperationType = OperationType.GroupChangeCommand;
                    args.UserUpdater = _userUpdater;
                    args.Update.Message!.Text = text.Replace("/change ", "").Trim();
                }

                return args;


            }
        }

        ICommandArgs ProcessNotExistingUser(ICommandArgs args)
        {
            bool innerCondition = args.User == null;

            if (!innerCondition) 
                return args;

            args = CheckMessageForGroupInput(args);
            args = TryGetStartCommand(args);

            return args;

            ICommandArgs CheckMessageForGroupInput(ICommandArgs args)
            {
                var text = args.Update.Message!.Text!.ToLower().Trim();

                bool firstCondition =
               !text.Contains(' ')
               && text.Contains('.')
               && text.Contains('/')
               && text.Contains('-')
               && text.Length >= 13
               && text.Length <= 19;

                bool secondCondition = text.StartsWith("97в/") || text.StartsWith("97з/");

                if (firstCondition || secondCondition)
                    args.OperationType = OperationType.GroupInput;

                return args;
            }
            ICommandArgs TryGetStartCommand(ICommandArgs args)
            {
                var text = args.Update.Message!.Text!;

                if (text is null)
                    return args;

                if (text.Split(' ').FirstOrDefault() == "/start")
                    args.OperationType = OperationType.StartCommand;

                return args;
            }
        }
        
    }
    private ICommandArgs ProcessCallback(ICommandArgs args)
    {

        if (args.Callback is null || args.Callback.Data is null)
            return args;

        if (args.Callback.Data.Contains("ScheduleSwitcher"))
        {
            args.OperationType = OperationType.SwitchWeekCallback;
            args.ScheduleLoader = _scheduleLoader;
        }

        args.CallbackMessageUpdater = _callbackMessageUpdater;
        args.UserUpdater = _userUpdater;

        return args;


    }

}