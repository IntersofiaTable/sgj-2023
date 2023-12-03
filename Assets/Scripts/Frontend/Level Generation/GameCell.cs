using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Frontend.EventProcessing;
using GameState;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelGeneration
{
    public class GameCell : Cell
    {
        public bool isMouseOver;

        public bool isActionPreview;

        public bool isHighlighted;

        public bool isPlayerControlled;

        public float HighlightAmount = 1.5f;

        public CellOcupationIcon cellIcon;

        public Color TargetColor => 
            isHighlighted       ? GameColors.Highlighted :
            isActionPreview     ? GameColors.Playable :
            isPlayerControlled  ? GameColors.PlayerControlled :
            GameColors.AIControlled;

        public Color FinalColor => isMouseOver
            ? new Color(TargetColor.r * HighlightAmount, TargetColor.g * HighlightAmount, TargetColor.b * HighlightAmount)
            : TargetColor;

        public Card Card { get; internal set; }

        public Color currentColor;

        public float offsetY = 0.4f;

        private List<Renderer> renderers;

        public void Start()
        {
            renderers = this.GetAllRenderers();
        }

        public void SetData(Card card)
        {
            Card = card;
            this.cellIcon.PopulateFromCard(card);
            this.cellIcon.transform.position = transform.position + new Vector3(0, this.GetCompleteBounds().size.y + offsetY, 0);
        }

        public void Update()
        {
            currentColor = new Color(
                Mathf.MoveTowards(currentColor.r, FinalColor.r, 2f * Time.deltaTime),
                Mathf.MoveTowards(currentColor.g, FinalColor.g, 2f * Time.deltaTime),
                Mathf.MoveTowards(currentColor.b, FinalColor.b, 2f * Time.deltaTime));
            
            renderers.ForEach(r => r.materials.ToList().ForEach(m => m.SetColor("_BaseColor", currentColor)));
            cellIcon.gameObject.SetActive(Card != null);
        }
    }
}