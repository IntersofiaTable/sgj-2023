using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace LevelGeneration
{
    public class GameCell : Cell
    {
        public Color targetColor;

        public bool isHighlighted;

        public float HighlightAmount = 1.5f;

        public Color FinalColor => isHighlighted
            ? new Color(targetColor.r * HighlightAmount, targetColor.g * HighlightAmount, targetColor.b * HighlightAmount)
            : targetColor;

        public Color currentColor;


        private List<Renderer> renderers;

        public void Start()
        {
            renderers = this.GetAllRenderers();
        }

        public void Update()
        {
                currentColor = new Color(
                    Mathf.MoveTowards(currentColor.r, FinalColor.r, 2f * Time.deltaTime),
                    Mathf.MoveTowards(currentColor.g, FinalColor.g, 2f * Time.deltaTime),
                    Mathf.MoveTowards(currentColor.b, FinalColor.b, 2f * Time.deltaTime));
                
                renderers.ForEach(r => r.materials.ToList().ForEach(m => m.SetColor("_BaseColor", currentColor)));
        }
    }
}