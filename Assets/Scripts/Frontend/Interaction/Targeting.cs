using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Frontend.Interaction;
using Frontend.EventProcessing;
using GameState.PlayerCommand;
using LevelGeneration;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Frontend.Interaction
{
    public class Targeting : MonoBehaviour
    {
        private GridGenerator gridGen;

        public GameObject cursor;

        public static Targeting Instance;

        public bool TargetingEnabled;

        public List<(int x, int y)> highlightedSpots = new List<(int x, int y)>();

        public IEnumerable<Cell> CellsToHighlight => highlightedSpots.Select(p => gridGen.GetCell(p.x, p.y)).ToList();

        private void Start()
        {
            Instance = this;
            gridGen = FindObjectOfType<GridGenerator>();
        }

        public void Update()
        {
            GetMouseInput();
            UpdateHighlighting();
        }

        private GameCell currentHighlightedCell;


        public void GetMouseInput()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var targetCell = gridGen.GetClosestCell(mouseRay);
            if (EventSystem.current.IsPointerOverGameObject())
            {
                ClearHighlight();
                return;

            }
            if (targetCell == null) { ClearHighlight(); return; }

                if (targetCell is GameCell gameCell)
            {
                if (gameCell != currentHighlightedCell)
                {
                    if (currentHighlightedCell != null)
                    {
                        currentHighlightedCell.isMouseOver = false;
                    }

                    currentHighlightedCell = gameCell;
                    gameCell.isMouseOver = true;

                }

                if (TargetingEnabled)
                {
                    if ( CardsController.Instance.SelectedCard != null)
                    {
                        var currentIdx = gridGen.GetCellPosition(gameCell);
                        highlightedSpots = CardsController.Instance.SelectedCard.ControlZone.Select(idx =>
                            (idx.X + currentIdx.x, currentIdx.y + idx.Y)).ToList();
                    }
                    else
                    {
                        highlightedSpots.Clear();
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        var pos = gridGen.GetCellPosition(gameCell);
                        CardsController.Instance.PlayCurrentCard(pos.x, pos.y);
                        TargetingEnabled = false;
                    }
                }
            }

            if (cursor != null)
            {
                cursor.transform.position = gridGen.GetTopOfGridCell(targetCell);
            }

            // Plane p = `
            // if(Physics.Raycast(mouseRay.origin, mouseRay.direction))

        }

        public void UpdateHighlighting()
        {
            var highlightSpots = CellsToHighlight.ToList();
            gridGen.GetAllCells().ForEach(c => {
                if (c is GameCell gc)
                {
                    gc.isHighlighted = highlightSpots.Contains(c);
                } });
        }

        private void ClearHighlight()
        {
            if (currentHighlightedCell != null)
            {
                currentHighlightedCell.isMouseOver = false;
            }
            cursor.transform.position = new Vector3(10000, 10000, 0);

        }

        public void SetAvailable((int X, int Y)[] validPlacements)
        {
            var cells = validPlacements.Select(vp => gridGen.GetCell(vp.X, vp.Y));
            foreach (var cell in cells )
            {
                if(cell is GameCell gameCell) 
                {
                    gameCell.isActionPreview = true;
                }
            };
        }

        public void ClearActionPlacement()
        {
            var hls = CellsToHighlight.ToList();
            gridGen.GetAllCells().ForEach(c => {
                if (c is GameCell gc)
                {
                    gc.isActionPreview = false;
                } });
        }

        public void ToggleOn()
        {
            TargetingEnabled = true;
        }
    }
}