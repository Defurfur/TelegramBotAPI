using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentCommandHandler;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="FluentCommandHandler{TSource, TResult}"/> implementation
    /// of <see cref="IFluentCommandHandler{TSource, TResult}"/> interface as a scoped DI 
    /// service with singletone <see cref="HandlerConfiguration{TSource, TResult}"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>S
    /// 
    public static void AddFluentCommandHandler<TSource, TResult>(
        this IServiceCollection services,
        Action<HandlerConfiguration<TSource, TResult>> options)
        where TSource : notnull
        where TResult : notnull
    {
        services.Configure(options);

        services.Add(new ServiceDescriptor(
            typeof(IFluentCommandHandler<TSource, TResult>),
            typeof(FluentCommandHandler<TSource, TResult>),
            ServiceLifetime.Scoped));
        
    }
}
