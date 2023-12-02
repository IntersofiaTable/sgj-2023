using GameState.PlayerCommand;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameState
{
    public class GameRules : GameRulesBase
    {
        private readonly int MapX = 10;
        private readonly int MapY = 12;
        private readonly int CardsToDraw = 3;

        private readonly IList<Card> babaCards;
        private readonly IList<Card> ingredientCards;

        private readonly Random random;
        private readonly Func<int, int, int> next;
        private readonly Func<int, int> nextFromZeroTo;

        public GameRules(IList<Card> allCards, Func<int, int, int> next = null)
        {
            this.babaCards = allCards.Where(x => x.Type == CardType.Baba).ToArray();
            this.ingredientCards = allCards.Where(x => x.Type == CardType.Ingredient).ToArray();

            random = new Random();
            this.next = next ?? random.Next;
            this.nextFromZeroTo = (i) => this.next(0, i);
        }

        public override void SetEmitter(IGameEventEmitter gameEventEmitter)
        {
            base.SetEmitter(gameEventEmitter);

        }

        public override (bool,GameState) Act(IPlayerCommand command, GameState state) =>
            command switch
                {
                    PlayerCommand.GetCardOptions o => GetCardOptions(o, state),
                    _ => (false, state)
                };

        public override (bool, GameState) TickTime(GameState state)
        {
            return (false, state);
        }

        public override (bool, GameState) StartGame(GameState state)
        {
            gameEventEmitter.Emit(new GameStartedEvent());
            (_, state) = LoadMap(state, 0);
            return (true, state);
        }

        private T GetNextFromList<T>(IList<T> collection) => collection[nextFromZeroTo(collection.Count)];

        private (bool, GameState) LoadMap(GameState state, int mapId)
        {
            {
                MapTile[][] g = Enumerable.Range(0, MapY)
                    .Select(y => Enumerable.Range(0, MapX)
                        .Select(x => new MapTile() { X = x, Y = y})
                        .ToArray())
                    .ToArray();

                MapState mapState = new MapState()
                {
                    Tiles = g,
                    X = MapX,
                    Y = MapY,
                };
                state.Map = mapState;
                state.BabaCount = 0;
                state.Turn = 0;
                state.HaveToDrawBaba = true;

                gameEventEmitter.Emit(new LoadMapEvent(mapState));
            }
            
            (_, state) = DrawCards(state);

            return (true, state);

        }

        private (bool, GameState) DrawCards(GameState state)
        {
            List<Card> draw = new List<Card>();

            if (state.HaveToDrawBaba)
            {
                draw.Add(GetNextFromList(babaCards));
                state.HaveToDrawBaba = false;
            }

            for (int i = 0; i < CardsToDraw; i++)
            {
                draw.Add(GetNextFromList(ingredientCards));
            }
                
            gameEventEmitter.Emit(new DrawCardsEvent(draw));

            return (true, state);
        }

        private (bool, GameState) GetCardOptions(GetCardOptions getCardOptions, GameState state)
        {
            Card card = getCardOptions.Card;
            if (card.Type == CardType.Ingredient)
            {
                var tiles = state.Map.Tiles
                    .SelectMany(x => x)
                    .Where(x => x.BabaControlled)
                    .Where(x => x.Card is null)
                    .Select(x => (x.X, x.Y));

                gameEventEmitter.Emit(new CardOptionsResponse(card, tiles.ToArray()));
            }

            if (card.Type == CardType.Baba)
            {
                List<(int X, int Y)> tiles = state.Map.Tiles
                    .SelectMany(x => x)
                    .Where(x => x.AIContolled) 
                    //.Where(x => x.Card is null)
                    .Select(x => (x.X, x.Y))
                    .ToList();

                List<(int X, int Y)> availableTiles = new List<(int X, int Y)>();
                foreach (var tile in tiles)
                {
                    bool isGood = true;
                    foreach (var control in card.ControlZone)
                    {
                        int checkY = tile.Y + control.Y;
                        if( checkY > MapY) continue;
                        if (checkY < 0) continue;

                        int checkX = tile.X + control.X;
                        if (checkX > MapX) continue;
                        if (checkX < 0) continue;

                        var checkTile = state.Map.Tiles[checkY][checkX];
                        if (checkTile.PlayerControlled)
                        {
                            isGood = false;
                            break;
                        }
                    }

                    if (isGood) availableTiles.Add(tile);
                }

                gameEventEmitter.Emit(new CardOptionsResponse(card, availableTiles.ToArray()));

            }

            return (false, state);
        }
    }
}