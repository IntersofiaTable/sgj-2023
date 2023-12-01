using GameState.PlayerCommand;

namespace GameState
{
    public interface IGameRules
    {
        public void SetEmitter(IGameEventEmitter gameEventEmitter);
        public (bool,GameState) Act(IPlayerCommand command, GameState state);
        public (bool,GameState) TickTime(GameState state);
        public (bool,GameState) StartGame(GameState state);
    }
    
    public abstract class GameRulesBase : IGameRules
    {
        protected IGameEventEmitter gameEventEmitter;
        
        public virtual void SetEmitter(IGameEventEmitter gameEventEmitter)
        {
            this.gameEventEmitter = gameEventEmitter;
        }

        public abstract (bool,GameState) Act(IPlayerCommand command, GameState state);

        public abstract (bool, GameState) TickTime(GameState state);

        public abstract (bool, GameState) StartGame(GameState state);
    }
}