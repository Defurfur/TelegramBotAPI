using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaSchedule.Models
{
    public class User: IIdentifyable<int>
    {
        public int Id { get; set; }
        public int ReaGroupId { get; set; }
        public ReaGroup? ReaGroup { get; set; }
        public long ChatId { get; set; }

    }
}
