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
                    PlayerCommand.PlaceCardCommand c => PlaceCard(c, state),
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
                state.BabasToDraw = 1;

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
                for (int i = 0; i < state.BabasToDraw; i++)
                {
                    draw.Add(GetNextFromList(babaCards));
                }
                state.BabasToDraw = 0;
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

            var tiles = GetCardOptions(card, state);
            gameEventEmitter.Emit(new CardOptionsResponse(card, tiles));

            return (false, state);
        }

        private (bool, GameState) PlaceCard(PlaceCardCommand command, GameState state)
        {
            var tileOptions = GetCardOptions(command.Card, state);
            bool isTileValid = tileOptions.Any(x => x.X == command.X && x.Y == command.Y);

            if (isTileValid == false) return (false, state);


            List<TileUpdate> updatedTiles = new List<TileUpdate>();

            var tile = state.Map.Tiles[command.Y][command.X];
            tile.Card = command.Card;
            if (command.Card.Type is CardType.Baba)
            {
                tile.BabaControlled = true;
                tile.ThisToBabaList.Add(tile);

                updatedTiles.Add(new TileUpdate(command.X, command.Y, true, command.Card));
                foreach (var control in command.Card.ControlZone)
                {
                    int checkY = tile.Y + control.Y;
                    if (checkY > MapY) continue;
                    if (checkY < 0) continue;

                    int checkX = tile.X + control.X;
                    if (checkX > MapX) continue;
                    if (checkX < 0) continue;

                    var controlTile = state.Map.Tiles[checkY][checkX];
                    controlTile.BabaControlled = true;
                    controlTile.ThisToBabaList.Add(tile);
                    tile.BabaIsControllingList.Add(controlTile);
                    updatedTiles.Add(new TileUpdate(checkX, checkY, true, null));

                }
            }
            if (command.Card.Type is CardType.Ingredient)
            {
                tile.IngredientControlled = true;
                foreach (var control in command.Card.ControlZone)
                {
                    int checkY = tile.Y + control.Y;
                    if (checkY > MapY) continue;
                    if (checkY < 0) continue;

                    int checkX = tile.X + control.X;
                    if (checkX > MapX) continue;
                    if (checkX < 0) continue;

                    var controlTile = state.Map.Tiles[checkY][checkX];
                    controlTile.IngredientControlled = true;

                    updatedTiles.Add(new TileUpdate(checkX, checkY, true, null));

                    foreach (var babaTile in controlTile.ThisToBabaList)
                    {
                        if (babaTile.BabaIsControllingList.All(x => x.Card != null))
                        {
                            state.BabasToDraw++;
                        }
                    }
                }
            }

            gameEventEmitter.Emit(new UpdateMapEvent(updatedTiles));

            return (true, state);
        }

        private (int X, int Y)[] GetCardOptions(Card card, GameState state)
        {
            if (card.Type == CardType.Ingredient)
            {
                var tiles = state.Map.Tiles
                    .SelectMany(x => x)
                    .Where(x => x.BabaControlled)
                    .Where(x => x.Card is null)
                    .Select(x => (x.X, x.Y));

                return tiles.ToArray();
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
                        if (checkY > MapY) continue;
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

                return availableTiles.ToArray();

            }

            return Array.Empty<(int,int)>();
        }

    }
}