namespace GameState
{
    public interface IGameEvent
    {
    }

    public sealed class GameStartedEvent : IGameEvent
    {

    }

    public sealed class GameEndedEvent : IGameEvent
    {
    }


    public sealed record LoadMapEvent(MapState MapState) : IGameEvent
    {
        
    }

    public sealed class DrawCardsEvent : IGameEvent
    {

    }


    public sealed class UpdateMapEvent : IGameEvent
    {

    }
}