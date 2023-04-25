using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdateService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services from ScheuleUpdateService project, which allow downloading and updating ReaGroup schedules
        /// </summary>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>S
        public static IServiceCollection AddScheduleUpdateService(this IServiceCollection services,
            ServiceLifetime lifetime)
        {
            services.Add(new ServiceDescriptor(typeof(IBrowserWrapper), typeof(BrowserWrapper), lifetime));
            services.Add(new ServiceDescriptor(typeof(IEntityUpdater), typeof(EntityUpdater), lifetime));
            services.Add(new ServiceDescriptor(typeof(IHashingService), typeof(HashingService), lifetime));
            services.Add(new ServiceDescriptor(typeof(IReaClassFactory), typeof(SimpleReaClassFactory), lifetime));
            services.Add(new ServiceDescriptor(typeof(IReaGroupFactory), typeof(ReaGroupFactory), lifetime));
            services.Add(new ServiceDescriptor(typeof(IScheduleParser), typeof(JsScheduleParser), lifetime));
            services.Add(new ServiceDescriptor(typeof(IScheduleWeekFactory), typeof(ScheduleWeekFactory), lifetime));
            services.Add(new ServiceDescriptor(typeof(IParserPipeline), typeof(ParserPipeline), lifetime));
            services.Add(new ServiceDescriptor(typeof(IChromiumKiller), typeof(ChromiumKiller), lifetime));
            return services;
        }
    }
}
