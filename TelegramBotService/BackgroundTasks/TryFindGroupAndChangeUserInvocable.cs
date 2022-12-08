using Coravel.Invocable;
using Humanizer;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Services;
using System.Diagnostics;
using TelegramBotService.Abstractions;
using TelegramBotService.Services;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotService.BackgroundTasks;




/// <summary>
/// Class that that implements <see cref="IInvocable"/> and <see cref="IInvocableWithPayload{T}"/> interfaces. 
/// It's used to process group input and search for a group schedule on a website as a queued background task.
/// Its only method <see cref="Invoke()"/> seeks for a group and if it's present - adds it to a database, 
/// changes <see cref="User"/> model and sends success message to a bot user. 
/// If it doesn't find any group - sends fail message.
/// </summary>

public class TryFindGroupAndChangeUserInvocable : IInvocable, IInvocableWithPayload<MessageAndUser>
{
    private readonly ILogger<TryFindGroupAndChangeUserInvocable> _logger;
    private readonly IParserPipeline _parserPipeline;
    private readonly IMessageSender _sender;
    private readonly IScheduleParser _scheduleParser;
    private readonly IContextUpdateService _contextUpdateService;
    public MessageAndUser Payload { get; set; }

    public TryFindGroupAndChangeUserInvocable(
        IParserPipeline parserPipeline,
        IMessageSender sender,
        IScheduleParser scheduleParser,
        IContextUpdateService contextUpdateService,
        ILogger<TryFindGroupAndChangeUserInvocable> logger)
    {
        _parserPipeline = parserPipeline;
        _sender = sender;
        _scheduleParser = scheduleParser;
        _contextUpdateService = contextUpdateService;
        _logger = logger;
    }


    /// <summary>
    /// Background Task used to be put in a queue. First, checks whether the group exists. If true - 
    /// parses it, updates db context, and sends success message to a user. If false - sends fail message. 
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <returns></returns>
    public async Task Invoke()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var groupAsString = Payload.Message.Text != null
            ? Payload.Message.Text.Replace("/change", "")
            : string.Empty;

        bool groupExists = false;

        ArgumentNullException.ThrowIfNull(Payload.Message, nameof(Payload.Message));
        ArgumentNullException.ThrowIfNull(Payload.User, nameof(Payload.User));
        ArgumentException.ThrowIfNullOrEmpty(groupAsString, nameof(groupAsString));
        try
        {
            groupExists = await _scheduleParser.CheckForGroupExistance(groupAsString);

            if (!groupExists)
            {
                await _sender.GroupNotFoundMessage(Payload.Message);
                return;
            }

            var group = await _contextUpdateService.CreateNewReaGroup(groupAsString);

            var updatedGroup = await _parserPipeline.ParseAndUpdate(group);

            if(updatedGroup.ScheduleWeeks is null)
            {
                await _sender.SomethingWentWrongMessage(Payload.Message);
                return;
            }

            await _contextUpdateService.UpdateReaGroup(
                group,
                updatedGroup.ScheduleWeeks.ToList(),
                updatedGroup.Hash);

            await _contextUpdateService.TryChangeUsersGroupAsync(Payload.User, groupAsString);

            await _sender.ChangeGroupSuccess(Payload.Message);

        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[Errors] {ExceptaionName} occurred while executing {ClassName}",
                ex.GetType().Name,
                GetType().Name);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "[Metrics] {ClassName} " +
                "with groupExists = {groupExists} took {Time} to finish",
                GetType().Name,
                groupExists,
                stopwatch.Elapsed.Humanize(2));
        }



    }

}
