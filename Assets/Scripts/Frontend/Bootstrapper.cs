using Frontend.EventProcessing;
using GameState;
using Unity.Entities;
using UnityEngine;

namespace Frontend
{
    public class Bootstrapper : MonoBehaviour
    {
        
        public void Awake()
        {
            var processorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameEventProcessorSystem>();
            var newGame = GameStateMachineFactory.GetDefaultMachine();
            processorSystem.BeginProcessing(newGame);
        }
    }
    
    public static class GameStateMachineFactory
    {
        public static GameStateMachine GetDefaultMachine()
        {
            var gameRules = new GameRules();  
            var gameState = new global::GameState.GameState()
            {
                Status = GameStatus.Preparing,
            };
            var machine = new GameStateMachine(gameRules, gameState);
            return machine;
        }
    }
}
