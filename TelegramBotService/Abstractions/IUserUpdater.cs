﻿using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IUserUpdater
    {
        Task<SubscriptionSettings> CreateSubscriptionSettings(User user);
        Task ProcessCallbackAndSaveChanges(SubscriptionSettings settings, string callbackData);
    }
}