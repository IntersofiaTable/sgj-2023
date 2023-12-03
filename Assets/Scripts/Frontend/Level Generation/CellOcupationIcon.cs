using Assets.Scripts.Frontend.Config;
using GameState;
using UnityEngine;
using UnityEngine.UI;

namespace LevelGeneration
{
    public class CellOcupationIcon : MonoBehaviour
    {
        public Image iconImg;
        public void PopulateFromCard(Card card)
        {
            if (card == null) return;
            var resource  = CardResources.Instance.GetCardResourceByID(card.Id);
            iconImg.sprite = resource.image;
        }
    }
}