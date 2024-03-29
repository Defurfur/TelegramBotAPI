﻿using System;
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
        Error
    }
    public enum OperationType
    {
        StartCommand,
        GroupInput,
        BugCommand,
        GroupChangeCommand,
        DownloadScheduleRequest,
        ChangeGroupButtonPressed,
        ChangeSubscriptionSettingsRequest,
        SwitchWeekCallback,
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
