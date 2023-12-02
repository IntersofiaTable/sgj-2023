using Cysharp.Threading.Tasks;
using Frontend.Interaction;
using GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Frontend.Interaction
{
    public class CardsController : MonoBehaviour
    {
        public static CardsController Instance;

        public List<Card> Cards = new List<Card>();

        private void Start()
        {
            Instance = this;
        }

        public async UniTask HandleCardsEvent(DrawCardsEvent drawEvt)
        {
            Debug.Log("Setting Cards.");
            Cards = drawEvt.Cards.ToList();
            Debug.Log("Cards + \n" + string.Join(" ", drawEvt.Cards.Select(c => $"ID : {c.Id} Name: {c.Name}\n")));
            await UniTask.Yield();
            Debug.Log("Cards Set.");
        }

        public async UniTask HandleCardUpdateReponse(CardOptionsResponse optionsResp)
        {
            Debug.Log("Updating Cards.");
            Targeting.Instance.SetAvailable(optionsResp.ValidPlacements);
            await UniTask.Yield();
            Debug.Log("Cards Updated.");
        }
    }
}
