namespace TelegramBot.Abstractions;

public interface IChainFactory<T>
{
    void CreateChain(List<T> chainMembers);
    T? GetFirstMember();
}
