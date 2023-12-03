using Assets.Scripts.Frontend.Interaction.UI;
using Cysharp.Threading.Tasks;
using Frontend.EventProcessing;
using Frontend.Interaction;
using GameState;
using GameState.PlayerCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Frontend.Interaction
{
    public class CardsController : MonoBehaviour
    {
        public static CardsController Instance;

        public static CardHand Hand;

        public Card SelectedCard;

        public List<Card> Cards = new List<Card>();

        private void Start()
        {
            Instance = this;
            Hand = FindObjectOfType<CardHand>();
        }

        public void SetSelectedCard(Card card)
        {
            if (Cards.Any(c => c.Id == card.Id))
            {
                Debug.Log("Card Selected");
                SelectedCard = Cards.First(c => c.Id == card.Id);
            }
            else
            {
                Debug.Log("Card NOT Selected");

            }
        }

        public void HandleCardsEvent(DrawCardsEvent drawEvt)
        {
            Debug.Log("Setting Cards.");
            Cards = drawEvt.Cards.ToList();
            Debug.Log("Drawing Cards.");
            Hand.DrawCards(Cards);
            Debug.Log("Cards + \n" + string.Join(" ", drawEvt.Cards.Select(c => $"ID : {c.Id} Name: {c.Name}\n")));
            //await UniTask.Yield();
            Debug.Log("Cards Set.");
        }

        public async UniTask HandleCardUpdateReponse(CardOptionsResponse optionsResp)
        {
            Debug.Log("Updating Cards.");
            Targeting.Instance.SetAvailable(optionsResp.ValidPlacements);
            await UniTask.Yield();
            Debug.Log("Cards Updated.");
        }

        public void PlayCurrentCard(int X, int Y)
        {
            if(SelectedCard == null) { Debug.Log("No Card Selected"); return; }
            var processorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameEventProcessorSystem>();
            processorSystem.Act(new PlaceCardCommand(X: X, Y: Y, Card: SelectedCard));
            CardHand.Instance.CommitCard(X, Y);
            Cards.Remove(SelectedCard);
        }

        public void RemoveCards(List<Card> toRemove)
        {
            if (toRemove == null || !toRemove.Any()) { return; }
            var remLst = new List<int>();
            if(this.Cards == null || this.Cards.Count == 0) { return; }
            for (int i = 0; i < toRemove.Count; i++)
            {
                for (int j = 0; j < Cards.Count; j++)
                {
                    if (Cards[j].Id.Equals(toRemove[i].Id))
                    {
                        remLst.Add(j);
                    }
                }
            }
            for (int i = 0; i < remLst.Count; i++)
            {
                Cards.RemoveAt(remLst[i]);
            }
        }

        public async UniTask HandleNewTurn()
        {
            await CardHand.Instance.DiscardHand();
        }

    }
}
