using Assets.Scripts.Frontend.Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Frontend.EventProcessing;
using Frontend.Interaction;
using GameState;
using GameState.PlayerCommand;
using JetBrains.Annotations;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Frontend.Interaction.UI
{
    public class CardHand : MonoBehaviour
    {
        public List<UICardBase> Cards = new List<UICardBase>();

        public static CardHand Instance;

        public Camera cam;

        public Transform camTarget;

        public Transform camNormalState;

        public Transform cardSelectedPos;

        public UICardBase IngredientCardPrefab;

        public UICardBase GrandmaCardPrefab;

        public Transform DiscardTransform;

        public Transform DrawPosition;

        public Transform Container;

        private LevelGenerator lvlGen;

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

        public async UniTask DiscardHand()
        {
            List<UniTask> tasks = new List<UniTask>();
            foreach (var card in Cards)
            {
                tasks.Add(card.transform.DOMove(DiscardTransform.position, AnimationTimes.DISCARD_HAND).AsyncWaitForCompletion().AsUniTask());
                tasks.Add(card.transform.DORotate(DiscardTransform.rotation.eulerAngles, AnimationTimes.DISCARD_HAND).AsyncWaitForCompletion().AsUniTask());
            }
            await UniTask.WhenAll(tasks);
            foreach (var c in Cards)
            {
                Destroy(c.gameObject);
            }
            Cards.Clear();
        }

        private void Start()
        {
            Instance = this;
            lvlGen = FindObjectOfType<LevelGenerator>();
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
                    item.SetState(UICardBase.CardState.Idle);
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
                var recessedTarget = new Vector3(offset * totalSpace, -475, MaxCards - i);

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
            if (state == UICardBase.CardState.Idle)
            {
                if(!Cards.Any(c => c.state != UICardBase.CardState.Idle))
                {
                    Targeting.Instance.ClearActionPlacement();
                }
            }

            if(state == UICardBase.CardState.Highlighted)
            {
                var processorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameEventProcessorSystem>();
                processorSystem.Act(new GetCardOptions(Card: card.data));
            }

            if(state == UICardBase.CardState.Pressed)
            {
                CardsController.Instance.SetSelectedCard(card.data);
                CardToPlay = card;
                Targeting.Instance.ToggleOn();
            }
        }

        public void HighlightCard()
        {
        }

        internal void UnselectAll()
        {
            var pressed = Cards.Where(c => c.state == UICardBase.CardState.Pressed);
            foreach (var item in pressed)
            {
                item.SetState(UICardBase.CardState.Idle);
            }
        }

        public async UniTask ConfirmPlay()
        {
            var tasks = new List<UniTask>();
            tasks.Add(CardToPlay.transform.DOMove(targetLerpPos, 0.8f).SetEase(Ease.InCubic).AsyncWaitForCompletion().AsUniTask());
            tasks.Add(CardToPlay.transform.DOLookAt(Vector3.forward, 0.3f).AsyncWaitForCompletion().AsUniTask());
            await UniTask.WhenAll(tasks);
            this.Cards.Remove(CardToPlay);
            Destroy(CardToPlay.gameObject);
            CardToPlay = null;
        }

        private UICardBase CardToPlay;

        private Vector3 targetLerpPos;

        internal void CommitCard(int x, int y)
        {
            targetLerpPos = lvlGen.GetTopOfGridCell(x, y);
        }

        public async UniTask AttackBoss()
        {
            var cells =lvlGen.GetAllCells();
            var cellsWithCards = cells.Select(c =>
            {
                if (c is GameCell gc)
                {
                    return gc;
                }

                return null;
            }).Where(gc => gc != null && gc.Card != null).ToList();

            int cnt = 0;
            Vector2 delayPerCard = new Vector2(0.22f, 0.04f);
            float totalDelay = 0;
            List<UniTask> tasks = new List<UniTask>();
            Vector2 violence = new Vector2(4f, 12f);
            foreach (var card in cellsWithCards)
            {
                var t = cnt / (float)cellsWithCards.Count();
                tasks.Add(card.cellIcon.transform.DOMove(AIController.Instance.healthBar.transform.position, 0.4f)
                    .SetDelay(totalDelay)
                    .SetEase(Ease.InCubic)
                    .OnComplete( () => Camera.main.DOShakePosition(0.1f, Mathf.Lerp(violence.x, violence.y, t)))
                    .AsyncWaitForCompletion()
                    .AsUniTask());
                totalDelay += Mathf.Lerp(delayPerCard.x, delayPerCard.y, t);
            }
            await UniTask.WhenAll(tasks);
        }
    }
}
