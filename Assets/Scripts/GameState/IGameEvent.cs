namespace GameState
{
    public interface IGameEvent
    {
    }

    public sealed class GameStartedEvent : IGameEvent
    {

    };

    public sealed class GameEndedEvent : IGameEvent
    {
    }

}