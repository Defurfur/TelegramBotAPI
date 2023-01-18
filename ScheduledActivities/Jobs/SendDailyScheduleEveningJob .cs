using Microsoft.Extensions.Logging;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using TelegramBotService.Abstractions;

namespace ScheduledActivities.Jobs
{
    public class SendDailyScheduleEveningJob : AbstractDailyScheduleJob
    {
        protected override ScheduleDbContext Context { get; set; }
        protected override IScheduleLoader Loader { get; set; }
        protected override IMessageSender Sender { get; set; }
        protected override ILogger<AbstractDailyScheduleJob> Logger { get; set; }
        protected override List<User>? Users { get; set; }
        protected override TimeOfDay TimeOfDay { get; set; } = TimeOfDay.Evening;

        public SendDailyScheduleEveningJob(
            ScheduleDbContext context,
            ILogger<AbstractDailyScheduleJob> logger,
            IScheduleLoader loader,
            IMessageSender sender)
        {
            Context = context;
            Loader = loader;
            Sender = sender;
            Logger = logger;
        }

    }
}
