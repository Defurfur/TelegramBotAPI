using AngleSharp.Browser;
using BenchmarkDotNet.Toolchains.Parameters;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftAntimalwareAMFilter;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Interfaces;
using TelegramBot.Models;

namespace TelegramBot.Services;

public interface IChainFactory<T>
{
    void CreateChain(List<T> chainMembers);
    T? GetFirstMember();
}

public interface ISpecificChainFactory : IChainFactory<IChainMember<ICommandArgs, Task<Message>>>
{

}
public class ChainFactory : ISpecificChainFactory
{
    private List<IChainMember<ICommandArgs, Task<Message>>>? _chainMembers;
    public void CreateChain(List<IChainMember<ICommandArgs, Task<Message>>> chainMembers)
    {
        if (chainMembers is null || !chainMembers.Any())
            return;

        for (var i = 0; i < chainMembers.Count - 1; i++)
        {
            chainMembers[i].SetNext(chainMembers[i + 1]);
        }

        _chainMembers = chainMembers;
    }

    public IChainMember<ICommandArgs, Task<Message>>? GetFirstMember()
    {
        if (_chainMembers is null || !_chainMembers.Any())
            return null;

        return _chainMembers.First();

    }
}
