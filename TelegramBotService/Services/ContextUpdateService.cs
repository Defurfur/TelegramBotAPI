﻿using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services
{
    public class ContextUpdateService : IContextUpdateService
    {
        private readonly ScheduleDbContext _context;

        public ContextUpdateService(ScheduleDbContext context)
        {
            _context = context;
        }

        public bool TryFindGroupInDb(string text, out ReaGroup? reaGroup)
        {
            text = text.Replace("/change ", "");

            var group = _context
                .ReaGroups
                .FirstOrDefault(x =>
                x!.GroupName == text.ToLower().Trim());

            reaGroup = group;
            return reaGroup != null;
        }

        public async Task TryRegisterUserAsync(ReaGroup group, long chatId)
        {
            var userExists = await _context
                .Users
                .AnyAsync(x => x.ReaGroupId == group.Id);

            if (userExists)
                return;

            var newUser = new ReaSchedule.Models.User()
            {
                ChatId = chatId,
                ReaGroup = group,
                ReaGroupId = group.Id,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }
        public async Task TryRegisterUserAsync(string groupName, long chatId)
        {
            var group = await _context
                .ReaGroups
                .FirstAsync(x => x.GroupName == groupName);

            if (group == null)
                return;

            await TryRegisterUserAsync(group, chatId);
        }



        public async Task TryChangeUsersGroupAsync(User user, ReaGroup reaGroup)
        {
            user.ReaGroup = reaGroup;
            user.ReaGroupId = reaGroup.Id;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

        }
        public async Task TryChangeUsersGroupAsync(User user, string groupName)
        {
            var group = await _context
               .ReaGroups
               .FirstAsync(x => x.GroupName == groupName);

            if (group == null)
                return;

            await TryChangeUsersGroupAsync(user, group);

        }


        /// <summary>
        /// Creates a new <see cref="ReaGroup"/> class with the given <paramref name="groupName"/>,  
        /// adds it to the context and returns. If a group with the given <paramref name="groupName"/> already
        /// exists - returns it.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<ReaGroup> CreateNewReaGroup(string groupName)
        {

            var group = await _context 
                .ReaGroups
                .FirstOrDefaultAsync(x => x.GroupName == groupName);

            if (group is not null)
                return group;

            group = new ReaGroup() { GroupName = groupName};

            _context.Add(group);
            await _context.SaveChangesAsync();

            return group;

        }

        public async Task UpdateReaGroup(
            ReaGroup group,
            List<ScheduleWeek> scheduleWeeks,
            string hash)
        {
            group.ScheduleWeeks = scheduleWeeks;
            group.Hash = hash;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeks for a group in context, which Id corresponds to a <paramref name="user"/>.ReaGroupId. If search 
        /// is succesfull, returns <see cref="ReaGroup"/>, otherwise - null.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ReaGroup?> DownloadUserScheduleAsync(User user)
        {
            var reaGroup = await _context
                .ReaGroups
                .Include(x => x.ScheduleWeeks!)
                    .ThenInclude(x => x.ScheduleDays)
                        .ThenInclude(x => x.ReaClasses)
                        .FirstOrDefaultAsync(x => x.Id == user.ReaGroupId);

            return reaGroup;
        }
    }
}
