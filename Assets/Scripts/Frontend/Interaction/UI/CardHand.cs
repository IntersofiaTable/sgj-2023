using Cysharp.Threading.Tasks;
using Frontend.EventProcessing;
using Frontend.Interaction;
using GameState;
using GameState.PlayerCommand;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Frontend.Interaction.UI
{
    public class CardHand : MonoBehaviour
    {
        public List<UICardBase> Cards = new List<UICardBase>();

        public CardHand Instance;

        public Camera cam;

        public Transform camTarget;

        public Transform camNormalState;

        public Transform cardSelectedPos;

        public UICardBase IngredientCardPrefab;

        public UICardBase GrandmaCardPrefab;

        public Transform DiscardTransform;

        public Transform DrawPosition;

        public Transform Container;

        public float RotSpeed = 800;

        public float MoveSpeed = 2500f;

        public float RotSpeedCam = 80;

        public float MoveSpeedCam = 250f;

        public float zStep = 10f;


        public int MaxCards = 10;
        public Vector2 CardOffsetRange = new Vector2(150, 40);
        public Quaternion CardRotationOffset = Quaternion.Euler(0,0,9);

        public void SetCards(List<Card> cards)
        {
        }

        public void DiscardHand()
        {

        }

        private void Start()
        {
            Instance = this;
        }

        public void DrawCards(List<Card> cards)
        {
            Debug.Log("Begining Draw");
            for (int i = 0; i < cards.Count; i++)
            {
                UICardBase instance = null;
                if (cards[i].Type == CardType.Baba) { instance = GameObject.Instantiate(GrandmaCardPrefab, Container);  }
                else { instance = GameObject.Instantiate(IngredientCardPrefab, Container); }
                Debug.Log("Instance Created");

                instance.Populate(cards[i]);
                instance.transform.position = DrawPosition.position;
                instance.transform.rotation = DrawPosition.rotation;
                instance.gameObject.SetActive(true);
                Debug.Log("Instance Populated");

                instance.CardStateChanged += (s) => CardStateChange(instance, s);
                Cards.Add(instance);
            }
        }

        public void Update()
        {
            PositionCards();
            ControlCamera();

            MonitorInput();
        }

        private void MonitorInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var pressed = Cards.Where(c => c.state == UICardBase.CardState.Pressed);
                foreach (var item in pressed)
                {
                    item.state = UICardBase.CardState.Idle;
                }
            }
        }

        public void PositionCards()
        {
            if(Cards == null || Cards.Count == 0) return;
            float totalSpace = 0;
            totalSpace += GrandmaCardPrefab.GetComponent<RectTransform>().sizeDelta.x;

            
            for (int i = 0; i < Cards.Count; i++)
            {
                totalSpace += Mathf.Lerp(CardOffsetRange.x, CardOffsetRange.y, i / MaxCards);
            }

            float normalSpace = 1f / Cards.Count;
            float bigSpace = 0.5f;
            if(Cards.Any(c => c.state == UICardBase.CardState.Highlighted) && Cards.Count > 2)
            {
                normalSpace = 1f / (Cards.Count + 1);
            }

            float centerPos = (Cards.Count-1) / 2f;

            for (int i = 0; i < Cards.Count; i++)
            {
                var t = (i - centerPos);
                
                var offset = t * normalSpace;

                var regularTarget = new Vector3(offset * totalSpace, 0, MaxCards - i);
                var recessedTarget = new Vector3(offset * totalSpace, -600, MaxCards - i);

                var anyPressed = Cards.Any(c => c.state == UICardBase.CardState.Pressed);
                var actualTarget = Cards[i].state == UICardBase.CardState.Pressed ? cardSelectedPos.localPosition :
                    anyPressed ? recessedTarget : 
                    regularTarget;

                var offsetRot = Quaternion.Euler(t * CardRotationOffset.eulerAngles.x, t * CardRotationOffset.eulerAngles.y, t * CardRotationOffset.eulerAngles.z);
                var rotTarget = anyPressed ? Quaternion.identity : offsetRot;

                var ap = Cards[i].GetComponent<RectTransform>().anchoredPosition;
                Cards[i].GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(ap, actualTarget, Time.deltaTime * MoveSpeed);
                var lp = Cards[i].GetComponent<RectTransform>().localPosition;
                Cards[i].GetComponent<RectTransform>().localPosition = new Vector3(lp.x, lp.y, (MaxCards - i ) * zStep) ;
                var rot = Cards[i].GetComponent<RectTransform>().localRotation;
                Cards[i].GetComponent<RectTransform>().localRotation = Quaternion.Slerp(rot, rotTarget, Time.deltaTime * RotSpeed);

            }

            //if (Cards.Any(c => c.state == UICardBase.CardState.Highlighted))
            //{
            //    var hlidx = Cards.FindIndex(c => c.state == UICardBase.CardState.Highlighted);
            //    for (int i = 0; i < Cards.Count; i++)
            //    {
            //        var d = Mathf.Abs(i - hlidx);
            //        Cards[i].GetComponent<RectTransform>().SetSiblingIndex(d);
            //    }
            //    Cards[hlidx].GetComponent<RectTransform>().SetAsLastSibling();
            //}
            //else
            //{

                for (int i = 0; i < Cards.Count; i++)
                { 
                    Cards[i].GetComponent<RectTransform>().SetSiblingIndex(i);
                }
            //}
        }

        public void ControlCamera()
        {
            var hasPressed = Cards.Any(c => c.state == UICardBase.CardState.Pressed);
            var ct = hasPressed ? camTarget : camNormalState;

            cam.transform.position = Vector3.MoveTowards(cam.transform.position, ct.position, Time.deltaTime * MoveSpeedCam);
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, ct.rotation, Time.deltaTime * RotSpeedCam);
        }


        public void CardStateChange(UICardBase card, UICardBase.CardState state)
        {
            if(state == UICardBase.CardState.Highlighted)
            {
                var processorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameEventProcessorSystem>();
                processorSystem.Act(new GetCardOptions(Card: card.data));
            }

            if(state == UICardBase.CardState.Pressed)
            {
                CardsController.Instance.SetSelectedCard(card.data);
                Targeting.Instance.ToggleOn();
            }
        }

        public void HighlightCard()
        {
        }
    }
}
