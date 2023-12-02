namespace GameState.PlayerCommand
{
    public interface IPlayerCommand { }

    public sealed record PlaceCardCommand(int X, int Y, Card Card) : IPlayerCommand { }

    public sealed record EndTurnCommand() : IPlayerCommand {}

    public sealed record GetCardOptions(Card Card) : IPlayerCommand { }
}