using Microsoft.Extensions.Logging;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotService.Abstractions;

namespace ScheduledActivities.Jobs
{
    public class SendDailyScheduleMorningJob : AbstractDailyScheduleJob
    {
        protected override ScheduleDbContext Context { get; set; }
        protected override IScheduleLoader Loader { get; set; }
        protected override IMessageSender Sender { get; set; }
        protected override ILogger<AbstractDailyScheduleJob> Logger { get; set; }
        protected override List<User>? Users { get; set; }
        protected override TimeOfDay TimeOfDay { get; set; } = TimeOfDay.Morning;
        protected override Exception? Exception { get; set; }


        public SendDailyScheduleMorningJob(
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
