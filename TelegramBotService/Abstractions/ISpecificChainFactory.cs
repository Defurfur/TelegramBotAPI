using Telegram.Bot.Types;

namespace TelegramBotService.Abstractions;

// review - suggestion - specific chain factory of what? Maybe ICommandArgsToMessageChainFactory or something? 
public interface ISpecificChainFactory : IChainFactory<IChainMember<ICommandArgs, Task<Message>>>
{

}
