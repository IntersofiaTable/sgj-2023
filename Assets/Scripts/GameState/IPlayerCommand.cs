namespace GameState.PlayerCommand
{
    public interface IPlayerCommand { }

    public sealed record PlaceCardCommand(int X, int Y, Card Card) {}
}