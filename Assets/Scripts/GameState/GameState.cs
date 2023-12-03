using System.Collections.Generic;
using System.Linq;

namespace GameState
{
    public class GameState
    {
        public GameStatus Status { get; set; }

        public int MapNumber { get; set; } = 0;
        public MapState Map { get; set; }

        public int Turn { get; set; }
        public int MaxTurn { get; set; } = 3;

        public int BabaCount { get; set; } = 0;
        public bool HaveToDrawBaba => BabasToDraw > 0;
        public int BabasToDraw { get; set; } = 1;

        public int AIHealth { get; set; } = 50;
        public int MaxMaps { get; set; } = 5;
    }

    public class MapState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public MapTile[][] Tiles { get; set; }

        public int PlayerControlled => Tiles.Select(x => x.Count(t => t.PlayerControlled)).Sum();
    }

    public class MapTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool AIContolled => !(BabaControlled || IngredientControlled);
        public bool PlayerControlled => BabaControlled || IngredientControlled;
        public bool BabaControlled { get; set; }
        public bool IngredientControlled { get; set; }
        public Card Card { get; set; }
        public List<MapTile> ThisToBabaList { get; set; } = new List<MapTile>();
        public List<MapTile> BabaIsControllingList { get; set; } = new List<MapTile>();
    }
}

