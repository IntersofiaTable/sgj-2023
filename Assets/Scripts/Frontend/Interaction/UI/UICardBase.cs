using Assets.Scripts.Frontend.Config;
using GameState;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Frontend.Interaction.UI
{
    public class UICardBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public TMP_Text TxtCardName;

        public Image ImgCard;

        public TMP_Text CardDescription;

        public CardState state;

        public event Action<CardState> CardStateChanged;

        public Card data;
        

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Selected");
            if (state == CardState.Idle || state == CardState.Highlighted)
            {
                state = CardState.Pressed;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("highlighted");
            if (state == CardState.Idle) {
                state = CardState.Highlighted;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("highlighted");
            if (state == CardState.Highlighted) {
                state = CardState.Idle;
            }
        }


        private void Update()
        {
            if (state != CardState.Idle)
            {
                transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                transform.localScale = Vector3.one ;
            }
        }

        internal void Populate(Card card)
        {
            data = card;
            var res = CardResources.Instance.GetCardResourceByID(card.Id);
            TxtCardName.text = res.name;
            ImgCard.sprite = res.image; 
        }


        public enum CardState
        {
            Idle,
            Highlighted,
            Pressed,
        }

    }
}
