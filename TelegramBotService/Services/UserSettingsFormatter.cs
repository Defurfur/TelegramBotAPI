using Humanizer;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services
{
    public class UserSettingsFormatter : IUserSettingsFormatter
    {
        private StringBuilder? _sb;
        private StringBuilder Sb { get => _sb ??= new(); }
        public string Format(SubscriptionSettings settings)
        {
            if (settings.DailySettingsValid)
            {
                FormatDailySubscriptionSettings(settings);
                EscapeCharacters();
                var result = Sb.ToString();
                Sb.Clear();
                return result ?? string.Empty;
            }

            if (settings.WeeklySettingsValid)
            {
                FormatWeeklySubscriptionSettings(settings);
                EscapeCharacters();
                var result = Sb.ToString();
                Sb.Clear();
                return result ?? string.Empty;
            }

            return string.Empty;
        }

        private void FormatDailySubscriptionSettings(SubscriptionSettings settings)
        {
            string includeTodayString = settings.IncludeToday
                ? "_да_"
                : "_нет (со следующего)_";

            string subscriptionState = settings.SubscriptionEnabled
                ? "Подписка: _включена_"
                : "Подписка: _выключена_";

            Sb.AppendLine(subscriptionState);
            Sb.AppendLine($"Обновлять раписание: _{settings.UpdateSchedule.Humanize()}_");
            Sb.AppendLine($"На: _{settings.DayAmountToUpdate.Humanize()}_");
            Sb.AppendLine("Включать день отправки: " + includeTodayString);
            Sb.AppendLine($"Присылать: _{settings.TimeOfDay.Humanize()}_");

        }
        private void FormatWeeklySubscriptionSettings(SubscriptionSettings settings)
        {
  
            string subscriptionState = settings.SubscriptionEnabled
                ? "Подписка: _включена_"
                : "Подписка: _выключена_";

            Sb.AppendLine(subscriptionState);
            Sb.AppendLine($"Присылать раписание: _{settings.UpdateSchedule.Humanize()}_");
            Sb.AppendLine($"На: _{settings.WeekToSend.Humanize()}_");
            Sb.AppendLine($"День обновления: _{settings.DayOfUpdate.Humanize()}_");
            Sb.AppendLine($"Присылать: _{settings.TimeOfDay.Humanize()}_");
        }
        private void EscapeCharacters()
        {
            Sb.Replace("""\""", """\\""");
            Sb.Replace(""".""", """\.""");
            Sb.Replace("""-""", """\-""");
            Sb.Replace("""+""", """\+""");
            Sb.Replace(""")""", """\)""");
            Sb.Replace("""(""", """\(""");
            Sb.Replace("""!""", """\!""");
            Sb.Replace(""":""", """\:""");
        }
    }
}
