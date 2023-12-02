using GameState.PlayerCommand;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameState
{
    public class GameRules : GameRulesBase
    {
        private readonly int MapX = 10;
        private readonly int MapY = 10;
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
            this.nextFromZeroTo = (i) => next(0, i);
        }

        public override void SetEmitter(IGameEventEmitter gameEventEmitter)
        {
            base.SetEmitter(gameEventEmitter);

        }

        public override (bool,GameState) Act(IPlayerCommand command, GameState state) =>
            command switch
                {

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
                MapTile[][] g = Enumerable.Range(0, MapX)
                    .Select(_ => Enumerable.Range(0, MapY)
                        .Select(_ => new MapTile())
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
    }
}