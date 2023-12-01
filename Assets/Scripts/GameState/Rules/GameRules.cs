using GameState.PlayerCommand;
using System;

namespace GameState
{
    public class GameRules : GameRulesBase
    {
        private readonly Random random;
        private readonly Func<int, int, int> next;

        
        public GameRules(Func<int, int, int> next = null)
        {
            random = new Random();
            this.next = next ?? random.Next;
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
            
            return (true, state);
        }
    }
}