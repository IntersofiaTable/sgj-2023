using System.Linq;

namespace GameState
{
    public class GameState
    {
        public GameStatus Status { get; set; }

        public MapState Map { get; set; }
        public int Turn { get; set; }
        public int MaxTurn { get; set; } = 10;
        public int BabaCount { get; set; } = 0;
    }

    public class MapState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public MapTile[][] Tiles { get; set; }

        public int PlayerControlled => Tiles.Select(x => x.Count(t => t.PlayerControlled)).Sum();
    }

    public struct MapTile
    {
        public readonly bool AIContolled => !(BabaControlled || IngredientControlled);
        public readonly bool PlayerControlled => BabaControlled || IngredientControlled;
        public bool BabaControlled { get; set; }
        public bool IngredientControlled { get; set; }
    }
}

