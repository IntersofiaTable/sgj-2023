using System;

namespace GameState
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CardType Type { get; set; }
        public (int X, int Y)[] ControlZone { get; set; } = Array.Empty<(int X, int Y)>();
    }

    public enum CardType
    {
        None,
        Baba,
        Ingredient
    }
}