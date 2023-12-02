using System.Collections.Generic;

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


    public sealed record LoadMapEvent(MapState MapState) : IGameEvent { }

    public sealed record DrawCardsEvent(ICollection<Card> Cards) : IGameEvent { }


    public sealed class UpdateMapEvent : IGameEvent
    {

    }

    public sealed record CardOptionsResponse(Card Card, (int X, int Y)[] ValidPlacements) : IGameEvent { }
}