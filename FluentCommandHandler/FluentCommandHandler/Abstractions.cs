using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FluentCommandHandler;

public interface IRuleSelectionStage<TSource, TResult> 
        where TSource : notnull
        where TResult : notnull
{
    /// <summary>
    /// Adds a condition for a new <typeparamref name="TResult"/> object.
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    ICommandCreationStage<TSource, TResult> AddRule(Func<TSource, bool> rule);

    /// <summary>
    /// Adds an async condition for a new <typeparamref name="TResult"/> object. <br/> Warning: avoid using this 
    /// method unless it's necessary, because it slows down the work of <see cref="FluentCommandHandler{TSource, TResult}"/>.
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    ICommandCreationStage<TSource, TResult> AddAsyncRule(Func<TSource, Task<bool>> rule);

}

public interface ICommandCreationStage<TSource, TResult> : IRuleSelectionStage<TSource, TResult>
     where TSource : notnull
     where TResult : notnull
{
    /// <summary>
    /// Maps <typeparamref name="TResult"/>'s constructor call to a rule or set of rules defined before. 
    /// </summary>
    /// <param name="command">Some</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    IRuleSelectionStage<TSource, TResult> ForCommand(Func<TSource, TResult> newCommandDelegate);
}
public interface IFluentCommandHandler<TSource, TResult>
    where TSource : notnull
    where TResult : notnull

{/// <summary>
/// Consecutively checks all the rules provided to the <see cref="HandlerConfiguration{TSource, TResult}"/>.
/// If one's check results in 'true' - executes <typeparamref name="TResult"/>'s
/// constructor call and returns a new object. <br/><br/> Thus, the order of the provided rules matters. 
/// If through all the rules there would be multiple rules that would've given 'true' - 
/// only the first <typeparamref name="TResult"/> would be returned.
/// </summary>
/// <param name="source"></param>
/// <returns><typeparamref name="TResult"/> if one of rules returns 'true'. 
/// <br/> <typeparamref name="null"/> if 'false'</returns>
    Task<TResult?> Process(TSource source);
}