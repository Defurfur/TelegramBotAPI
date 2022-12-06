using Coravel.Invocable;
using PuppeteerSharp;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Services;
using TelegramBotService.Abstractions;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotService.BackgroundTasks
{



    /// <summary>
    /// Class that that implements <see cref="IInvocable"/> and <see cref="IInvocableWithPayload{T}"/> interfaces. 
    /// It's used to process group input and search for a group schedule on a website as a queued background task.
    /// Its only method <see cref="Invoke()"/> seeks for a group and if it's present - adds it to a database, creates new
    /// <see cref="User"/> and sends success message to a bt user. If it doesn't find any group - sends fail message.
    /// </summary>

    public class TryFindGroupAndRegisterUserInvocable : IInvocable, IInvocableWithPayload<Message>
    {
        private readonly IParserPipeline _parserPipeline;
        private readonly IMessageSender _sender;
        private readonly IContextUpdateService _contextUpdateService;
        private readonly IScheduleParser _scheduleParser;
        public Message Payload { get; set; }

        public TryFindGroupAndRegisterUserInvocable(
            IParserPipeline parserPipeline,
            IMessageSender sender,
            IScheduleParser scheduleParser,
            IContextUpdateService contextUpdateService)
        {
            _parserPipeline = parserPipeline;
            _sender = sender;
            _scheduleParser = scheduleParser;
            _contextUpdateService = contextUpdateService;
        }


        /// <summary>
        /// Background task used to be put in a queue. First, checks whether the group exists. If true - 
        /// parses it, updates db context, and sends success message to a user. If false - sends fail message. 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public async Task Invoke()
        {
            var groupAsString = Payload.Text != null ? Payload.Text.Replace("/change", "") : string.Empty;

            ArgumentNullException.ThrowIfNull(Payload, nameof(Payload));
            ArgumentException.ThrowIfNullOrEmpty(groupAsString, nameof(groupAsString));

            var groupExists = await _scheduleParser.CheckForGroupExistance(groupAsString);

            if (groupExists)
            {
                await _contextUpdateService.ParseScheduleWebsiteAndAddToContextNew(
                  groupName:             groupAsString,
                  parseAndUpdateMethod: _parserPipeline.ParseAndUpdate);

                await _contextUpdateService.TryRegisterUserAsync(groupAsString, Payload.Chat.Id);

                await _sender.GroupFoundMessage(Payload);

                return;
            }

            await _sender.GroupNotFoundMessage(Payload);
            return;

        }

        
        //private async Task ParseScheduleWebsiteAndAddToContextOld(string groupName)
        //{
        //    if (_context.ReaGroups.Any(x => x.GroupName == groupName))
        //        return;


        //    var group = new ReaGroup() { GroupName = groupName };

        //    _context.Add(group);
        //    //_context.Add(new ReaGroup() { GroupName = groupName });

        //    await _context.SaveChangesAsync();

        //    //var createdGroup = _context
        //    //    .ReaGroups
        //    //    .First(x => x.GroupName == groupName);

        //    group = await _parserPipeline.ParseAndUpdate(group);
        //    //var updatedReaGroup = await _parserPipeline.ParseAndUpdate(group);

        //    //createdGroup.ScheduleWeeks = updatedReaGroup.ScheduleWeeks;
        //    //createdGroup.Hash = updatedReaGroup.Hash;

        //    await _context.SaveChangesAsync();


        //    //Check new version
        //}

    }
}
