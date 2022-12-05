using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotService.Models
{
    public enum GroupSearchState
    {
        FoundInDatabase,
        InProcess,
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
