using ReaSchedule.DAL;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Services;

public class UserUpdater : IUserUpdater
{
    private readonly ScheduleDbContext _context;
 


    public UserUpdater(ScheduleDbContext context)
    {
        _context = context;
    }
    public async Task<SubscriptionSettings> CreateSubscriptionSettings(User user)
    {
        if (user.SubscriptionSettings != null)
            return user.SubscriptionSettings;

        user.SubscriptionSettings = new() {SubscriptionEnabled = true};

        _context.Update(user);
        await _context.SaveChangesAsync();

        return user.SubscriptionSettings;
    }
    public async Task ProcessCallbackAndSaveChanges(SubscriptionSettings settings, string callbackData)
    {
        if (callbackData == null || callbackData == string.Empty || settings == null)
            return;

        _ = DictionaryStorage.CallbackSettingsActionsDictionary
                .TryGetValue(callbackData, out var action);

        action?.Invoke(settings);

        _context.Update(settings);
        await _context.SaveChangesAsync();
       
        }
    }


