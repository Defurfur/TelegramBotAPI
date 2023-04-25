using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaSchedule.Models;

public class Bug : IIdentifyable<int>
{
    public int Id { get; set; }
    public int UserId { get; set; } = 0;
    public long ChatId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Resolved { get; set; }

}
