using Microsoft.Extensions.Options;

namespace FluentCommandHandler;

public class FluentCommandHandler<TSource, TResult> : IFluentCommandHandler<TSource, TResult>
    where TSource : notnull
    where TResult : notnull
{
    private readonly IOptions<HandlerConfiguration<TSource, TResult>> _options;

    public FluentCommandHandler(
        IOptions<HandlerConfiguration<TSource, TResult>> options)
    {
        _options = options;
    }

    public async Task<TResult?> Process(TSource source)
    {
        var config = _options.Value;

        if(config.NotConfigured)
            throw new ArgumentException("HandlerConfiguration occured to not have been configured", nameof(config));

        config.OptimizeConfiguration();

        var result = await InnerProcess(config, source);

        return result;
    }

    private async Task<TResult?> InnerProcess(
        HandlerConfiguration<TSource, TResult> config,
        TSource source)
    {
        var result = config.ConfigurationDetails switch
        {
            ConfigurationDetails.WithAsyncMultiple => await ProcessAsyncMultiple(config, source),
            ConfigurationDetails.WithAsyncSingle => await ProcessAsyncSingle(config, source),
            ConfigurationDetails.WithSyncSingle => ProcessSyncSingle(config, source),
            _ => default
        };

        return result;
    }
    private async Task<bool> ProcessConditionItemList(
        List<ConditionItem<TSource>> items,
        TSource source)
    {
        foreach(var item in items)
        {
            if (!item.IsValid)
                throw new ArgumentException("An attempt to process an invalid ConditionItem", nameof(item));

            var result = item.AsyncCondition is null
                ? item.SyncCondition!.Invoke(source)
                : await item.AsyncCondition.Invoke(source);

            if(result is false)
                return result;
        }

        return true;
    }

    private async Task<TResult?> ProcessAsyncMultiple(
        HandlerConfiguration<TSource, TResult> config,
        TSource source)
    {
        foreach (var (commandFunc, conditionItems) in config.CommandAndConditionItemList)
        {
            if (await ProcessConditionItemList(conditionItems, source) is true)
                return commandFunc.Invoke(source);
        }

        return default;
    }

    private async Task<TResult?> ProcessAsyncSingle(
        HandlerConfiguration<TSource, TResult> config,
        TSource source)
    {
        var innerCondition = false;

        foreach (var (commandFunc, item) in config.CommandAndConditionItemDict)
        {
            innerCondition = item.AsyncCondition is null
                ? item.SyncCondition!.Invoke(source)
                : await item.AsyncCondition.Invoke(source);

            if (innerCondition is true)
                return commandFunc.Invoke(source);
        }

        return default;
    }
    private TResult? ProcessSyncSingle(
        HandlerConfiguration<TSource, TResult> config,
        TSource source)
    {
        foreach (var (commandFunc, condition) in config.CommandAndFuncDict)
        {
            if (condition.Invoke(source) is true)
                return commandFunc.Invoke(source);
        }

        return default;
    }
}
