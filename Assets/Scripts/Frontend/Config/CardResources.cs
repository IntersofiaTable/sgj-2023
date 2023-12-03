using Frontend.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Frontend.Config
{
    public class CardResources : MonoBehaviour
    {

        public CardVisualConfig CardConfig;

        public static CardResources Instance;

        private void Start()
        {
            Instance = this;
        }

        public CardVisualData GetCardResourceByID(int id)
        {
            if(CardConfig != null)
            {
                if(CardConfig.CardData.TryGetValue(id, out var card))
                {
                    return card;
                }

            }
            return null;
        }
    }
}
