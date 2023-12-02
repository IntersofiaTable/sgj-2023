using GameState.PlayerCommand;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameState
{
    public class GameRules : GameRulesBase
    {
        private int MapX = 10;
        private int MapY = 10;

        private readonly Random random;
        private readonly Func<int, int, int> next;
        private readonly Func<int, int> nextFromZeroTo;

        public GameRules(Func<int, int, int> next = null)
        {
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
            LoadMap(0);
            return (true, state);
        }


        private T GetNextFromList<T>(IList<T> collection) => collection[nextFromZeroTo(collection.Count)];


        private void LoadMap(int mapId)
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

                gameEventEmitter.Emit(new LoadMapEvent(mapState));
            }

            {

            }

        }
    }
}