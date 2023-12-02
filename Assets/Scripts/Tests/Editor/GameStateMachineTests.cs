using System;
using GameState;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using GameState.PlayerCommand;

namespace Tests.Editor
{
    public class GameStateMachineTests
    {
        private Card babaCard1 = new Card()
        {
            Id = 1,
            Name = "baba1",
            Type = CardType.Baba,
            ControlZone = new[] { (0, -1) }
        };
        private Card ingCard1 = new Card()
        {
            Id = 50,
            Name = "ing1",
            Type = CardType.Ingredient,
            ControlZone = new (int,int)[] { }
        };
        private Card ingCard2 = new Card()
        {
            Id = 51,
            Name = "ing2",
            Type = CardType.Ingredient,
            ControlZone = new (int,int)[] { }
        };
        private Card ingCard3 = new Card()
        {
            Id = 53,
            Name = "ing3",
            Type = CardType.Ingredient,
            ControlZone = new (int,int)[] { }
        };
        
        [Test]
        public void StartNewGame_GiveProperNewGameEvents()
        {
            var game = new GameStateMachine(new GameRules(
                new List<Card>()
                {
                    babaCard1,
                    ingCard1,
                    ingCard2,
                    ingCard3
                }, new GameOptions()
                {
                    CardsToDraw = 1,
                    MapX = 2,
                    MapY = 2
                })
                ,
                new GameState.GameState()
                {
                   Status = GameStatus.Preparing,
                });
            
            game.Start();


            ICollection<Card> playCards = Array.Empty<Card>();
            
            AssertExtender.Collection(game.EventQueue.ToArray(),
                evt =>
                    {
                        Assert.IsInstanceOf<GameStartedEvent>(evt.Event);
                    },
                evt =>
                {
                    Assert.IsInstanceOf<LoadMapEvent>(evt.Event);
                },evt =>
                {
                    Assert.IsInstanceOf<DrawCardsEvent>(evt.Event);
                    var drawCardsEvent = evt.Event as DrawCardsEvent;

                    playCards = drawCardsEvent.Cards;
                    Assert.AreEqual(drawCardsEvent.Cards.Count(x => x.Type == CardType.Baba), 1);
                    Assert.AreEqual(drawCardsEvent.Cards.Count(x => x.Type == CardType.Ingredient), 1);
                }
            );
            game.EventQueue.Clear();
            game.Act(new GetCardOptions(babaCard1));
            AssertExtender.Collection(game.EventQueue.ToArray(),
                evt =>
                {
                    Assert.IsInstanceOf<CardOptionsResponse>(evt.Event);
                    var cardOptionsResponse = evt.Event as CardOptionsResponse;

                });
            game.EventQueue.Clear();

            game.Act(new PlaceCardCommand(0, 1, babaCard1));
            AssertExtender.Collection(game.EventQueue.ToArray(),                
                evt =>
            {
                Assert.IsInstanceOf<UpdateMapEvent>(evt.Event);
                var updateMapEvent = evt.Event as UpdateMapEvent;

            });
            game.EventQueue.Clear();

            game.Act(new EndTurnCommand());
            AssertExtender.Collection(game.EventQueue.ToArray(),                
                evt =>
                {
                    Assert.IsInstanceOf<TurnUpdateEvent>(evt.Event);
                    var turnUpdateEvent = evt.Event as TurnUpdateEvent;

                },evt =>
                {
                    Assert.IsInstanceOf<DrawCardsEvent>(evt.Event);
                    var drawCardsEvent = evt.Event as DrawCardsEvent;
                    playCards = drawCardsEvent.Cards;
                });
            game.EventQueue.Clear();

            
            game.Act(new GetCardOptions(playCards.First()));
            AssertExtender.Collection(game.EventQueue.ToArray(),
                evt =>
                {
                    Assert.IsInstanceOf<CardOptionsResponse>(evt.Event);
                    var cardOptionsResponse = evt.Event as CardOptionsResponse;

                    Assert.AreEqual(cardOptionsResponse.ValidPlacements.Single(), (0, 0));
                });
            game.EventQueue.Clear();

            game.Act(new PlaceCardCommand(0, 0, playCards.First()));
            AssertExtender.Collection(game.EventQueue.ToArray(),                
                evt =>
                {
                    Assert.IsInstanceOf<UpdateMapEvent>(evt.Event);
                    var updateMapEvent = evt.Event as UpdateMapEvent;
                });
            game.EventQueue.Clear();

            game.Act(new EndTurnCommand());
            AssertExtender.Collection(game.EventQueue.ToArray(),                
                evt =>
                {
                    Assert.IsInstanceOf<TurnUpdateEvent>(evt.Event);
                    var turnUpdateEvent = evt.Event as TurnUpdateEvent;

                },evt =>
                {
                    Assert.IsInstanceOf<DrawCardsEvent>(evt.Event);
                    var drawCardsEvent = evt.Event as DrawCardsEvent;
                    playCards = drawCardsEvent.Cards;
                });
            game.EventQueue.Clear();
        }
    }
}