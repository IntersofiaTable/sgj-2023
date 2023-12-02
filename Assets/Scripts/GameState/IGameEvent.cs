using System.Collections.Generic;

namespace GameState
{
    public interface IGameEvent { }

    public sealed record GameStartedEvent : IGameEvent { }

    public sealed record GameEndedEvent(bool Win) : IGameEvent { }

    public sealed record LoadMapEvent(MapState MapState, int MapNumber) : IGameEvent { }
    public sealed record EndMapEvent(int AIHealth) : IGameEvent { }

    public sealed record DrawCardsEvent(ICollection<Card> Cards) : IGameEvent { }

    public sealed record UpdateMapEvent(ICollection<TileUpdate> UpdatedTiles) : IGameEvent { }

    public sealed record TileUpdate(int X, int Y, bool PlayerInControl, Card card) {}

    public sealed record CardOptionsResponse(Card Card, (int X, int Y)[] ValidPlacements) : IGameEvent { }

    public sealed record TurnUpdateEvent(int Turn): IGameEvent {}
}