using Cysharp.Threading.Tasks;
using Frontend.Config;
using Frontend.EventProcessing;
using GameState;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Frontend
{
    public class Bootstrapper : MonoBehaviour
    {
        public CardVisualConfig CardVisualConfig;

        public void Awake()
        {
            var processorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameEventProcessorSystem>();
            var newGame = GameStateMachineFactory.GetDefaultMachine(CardVisualConfig);
            processorSystem.BeginProcessing(newGame);
        }
    }
    
    public static class GameStateMachineFactory
    {
        public static GameStateMachine GetDefaultMachine(CardVisualConfig cardVisualConfig)
        {
            var cards = cardVisualConfig.CardData.Select(kvp => new Card()
            {
                Id = kvp.Key,
                Type = kvp.Value.cardType,
                Name = kvp.Value.name,
                ControlZone = kvp.Value.area.Select(x => ((int)x.x, (int)x.y)).ToArray()
            }).ToArray();

            var gameRules = new GameRules(cards, new GameOptions()
            {
                
            });  
            var gameState = new global::GameState.GameState()
            {
                Status = GameStatus.Preparing,
            };
            var machine = new GameStateMachine(gameRules, gameState);
            return machine;
        }
    }
}
