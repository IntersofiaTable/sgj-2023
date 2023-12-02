using GameState;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Editor
{
    public class GameStateMachineTests
    {
        [Test]
        public void StartNewGame_GiveProperNewGameEvents()
        {
            var game = new GameStateMachine(new GameRules(new List<Card>()), new GameState.GameState()
                {
                   Status = GameStatus.Preparing,
                });
            
            game.Start();
            
            AssertExtender.Collection(game.EventQueue.ToArray(),
                evt =>
                    {
                        Assert.IsInstanceOf<GameStartedEvent>(evt.Event);
                    }
            );
        }
    }
}