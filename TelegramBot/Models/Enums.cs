using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public enum GroupHasBeenFound
    {
        InDatabase,
        InSchedule,
        False,
    }
    public enum OperationType
    {
        IsStartCommand,
        IsGroupInput,
        GroupChangeCommand,
        DownloadScheduleRequest,
        ChangeGroupButtonPressed,
        ChangeSubscriptionSettingsRequest,
        Other,
    }

    public enum CallbackType
    {
        None,
        NeedsUserUpdate,
        WaitsForAnotherKeyboard,
        ChangeScheduleFrequency
    }
}
