using GameState.PlayerCommand;
using System;
using System.Collections.Generic;

namespace GameState
{
    public interface IGameEventEmitter
    {
        public void Emit(IGameEvent gameEvent);
        public GameState State { get; }
    }

    public interface ITimeTickReceiver
    {
        public void Tick();
    }

    public class GameStateMachine: IGameEventEmitter, ITimeTickReceiver
    {
        public GameStateMachine(IGameRules gameRules, GameState gameState)
        {
            this.gameRules = gameRules;
            gameRules.SetEmitter(this);
            State = gameState;
        }

        private int eventCount = 0;
        
        public Queue<GameEventArgs> EventQueue { get; } = new();
        public GameState State { get; private set; }
        private IGameRules gameRules { get; set; }

        public void Start()
        {
            if (State.Status != GameStatus.Preparing) throw new Exception("Game can only be started in preparing mode");
            
            State.Status = GameStatus.Started;
            gameRules.StartGame(State);
        }

        public virtual void Emit(IGameEvent gameEvent)
        {
            eventCount++;
            EventQueue.Enqueue(new GameEventArgs(gameEvent, eventCount));
        }
        
        public bool Act(IPlayerCommand command)
        {
            if (State.Status != GameStatus.Started) return false;
            
            var (can, state) = gameRules.Act(command, State);
            State = state;
            return can;
        }

        public void Tick()
        {
            gameRules.TickTime(State);
        }
    }
    
    public class GameEventArgs : EventArgs
    {
        public GameEventArgs(IGameEvent gameEvent, int id)
        {
            Event = gameEvent;
            Id = id;
        }
        
        public IGameEvent Event { get; set; }
        public int Id { get; set; }
    }
}