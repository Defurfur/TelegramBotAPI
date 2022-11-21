using Telegram.Bot.Types;

namespace TelegramBotService.Abstractions;

public interface ISpecificChainFactory : IChainFactory<IChainMember<ICommandArgs, Task<Message>>>
{

}
