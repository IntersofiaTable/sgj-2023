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

        public void HandleCardsEvent(DrawCardsEvent drawEvt)
        {
            Cards = drawEvt.Cards.ToList();
        }

        public void HandleCardUpdateReponse(CardOptionsResponse optionsResp)
        {
            Targeting.SetAvailable(optionsResp.ValidPlacements);

        }
    }
}
