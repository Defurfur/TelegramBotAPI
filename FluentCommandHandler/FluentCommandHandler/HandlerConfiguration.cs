using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FluentCommandHandler;

public enum ConfigurationDetails
{
    NotConfigured,
    WithAsyncSingle,
    WithAsyncMultiple,
    WithSyncSingle,
    WithSyncMultiple,
}
/// <summary>
/// Configuration for <see cref="FluentCommandHandler{TSource, TResult}"/>
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IHandlerConfiguration<TSource, TResult> :
    IRuleSelectionStage<TSource, TResult>,
    ICommandCreationStage<TSource, TResult>
    where TSource : notnull
    where TResult : notnull
{

}

/// <inheritdoc cref="IHandlerConfiguration{TSource, TResult}"/>
public class HandlerConfiguration<TSource, TResult> : IHandlerConfiguration<TSource, TResult>
    where TResult : notnull
    where TSource : notnull

{
    List<ConditionItem<TSource>> _conditionItems = new();
    internal ConfigurationDetails ConfigurationDetails { get; private set; } = ConfigurationDetails.NotConfigured;
    internal bool NotConfigured => ConfigurationDetails == ConfigurationDetails.NotConfigured;
    internal bool HasAsyncRules { get; private set; } = false;
    internal bool HasBeenOptimized { get; private set; } = false;

    internal Dictionary<Func<TSource, TResult>, List<ConditionItem<TSource>>> CommandAndConditionItemList { get =>
            _commandAndConditionItemList ??= new(); }
    internal Dictionary<Func<TSource, TResult>, ConditionItem<TSource>> CommandAndConditionItemDict { get =>
            _commandAndConditionItemDict ??= new(); }
    internal Dictionary<Func<TSource, TResult>, Func<TSource, bool>> CommandAndFuncDict { get =>
            _commandAndFuncDict ??= new(); }

    Dictionary<Func<TSource, TResult>, List<ConditionItem<TSource>>>? _commandAndConditionItemList;
    Dictionary<Func<TSource, TResult>, ConditionItem<TSource>>? _commandAndConditionItemDict;
    Dictionary<Func<TSource, TResult>, Func<TSource, bool>>? _commandAndFuncDict;

    
    public IRuleSelectionStage<TSource, TResult> ForCommand(Func<TSource, TResult> command)
    {
        if (_conditionItems.Count == 0)
            throw new ArgumentException("Did not provide rules for command", nameof(command));

        CommandAndConditionItemList.Add(command, _conditionItems);

        ConfigurationDetails = ConfigurationDetails.WithAsyncMultiple;

        _conditionItems = new();

        return this;
    }

    /// <summary>
    /// Adds a condition for a new <typeparamref name="TResult"/> object.
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    public ICommandCreationStage<TSource, TResult> AddRule(Func<TSource, bool> rule)
    {
        _conditionItems.Add(new ConditionItem<TSource> { SyncCondition = rule });

        return this;
    }

    public ICommandCreationStage<TSource, TResult> AddAsyncRule(Func<TSource, Task<bool>> asyncRule)
    {
        _conditionItems.Add(new ConditionItem<TSource> { AsyncCondition = asyncRule });
        HasAsyncRules = true;

        return this;
    }



    /// <summary>
    /// When first time <see cref="FluentCommandHandler{TSource, TResult}.Process(TSource)"/> executes, 
    /// this method is executed as well. Since <see cref="HandlerConfiguration{TSource, TResult}"/> methods 
    /// configure it the most non-optimized way, it's aim is to optimize it if there is a possibility.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    internal void OptimizeConfiguration()
    {
        if (HasBeenOptimized == true)
            return;

        if (ConfigurationDetails != ConfigurationDetails.WithAsyncMultiple)
            return;

        if (CommandAndConditionItemList is null || !CommandAndConditionItemList.Any())
            throw new ArgumentException("Configuration contains zero entries");

        bool listsCanBeAvoided = true;

        foreach (var (key, valueList) in CommandAndConditionItemList)
        {
            if (valueList.Count != 1)
            {
                listsCanBeAvoided = false;
                break;
            }
        }

        if (listsCanBeAvoided)
            AvoidListsOptimization();

        if (ConfigurationDetails == ConfigurationDetails.WithAsyncSingle && !HasAsyncRules)
            TurnToSyncOptimization();

        HasBeenOptimized = true;
    }
    /// <summary>
    /// Optimization method, used if storing Lists in 
    /// <see cref="HandlerConfiguration{TSource, TResult}"/> can be avoided.
    /// </summary>
    private void AvoidListsOptimization()
    {
        foreach(var (key, value) in CommandAndConditionItemList)
        {
            CommandAndConditionItemDict[key] = value.First();
        }

        CommandAndConditionItemList.Clear();

        ConfigurationDetails = ConfigurationDetails.WithAsyncSingle;
    }

    private void TurnToSyncOptimization()
    {
        foreach (var (key, value) in CommandAndConditionItemDict)
        {
            CommandAndFuncDict[key] = value.SyncCondition 
                ?? throw new ArgumentException("Dictionary contains async conditions");
        }

        CommandAndConditionItemDict.Clear();
        ConfigurationDetails= ConfigurationDetails.WithSyncSingle;
    }

}
