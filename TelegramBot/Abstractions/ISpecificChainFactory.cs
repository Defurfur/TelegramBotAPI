using Telegram.Bot.Types;

namespace TelegramBot.Abstractions;

public interface ISpecificChainFactory : IChainFactory<IChainMember<ICommandArgs, Task<Message>>>
{

}
