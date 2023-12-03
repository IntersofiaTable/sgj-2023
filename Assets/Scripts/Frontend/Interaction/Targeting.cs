using System;
using System.Linq;
using Assets.Scripts.Frontend.Interaction;
using Frontend.EventProcessing;
using GameState.PlayerCommand;
using LevelGeneration;
using Unity.Entities;
using UnityEngine;

namespace Frontend.Interaction
{
    public class Targeting : MonoBehaviour
    {
        private GridGenerator gridGen;

        public GameObject cursor;

        public static Targeting Instance;

        public bool TargetingEnabled;
        
        private void Start()
        {
            Instance = this;
            gridGen = FindObjectOfType<GridGenerator>();
        }

        public void Update()
        {
            GetMouseInput();
        }

        private GameCell currentHighlightedCell;


        public void GetMouseInput()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var targetCell = gridGen.GetClosestCell(mouseRay);
            if(targetCell == null) { return; }
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

                //if (TargetingEnabled)
                //{
                    if (Input.GetMouseButtonDown(0))
                    {
                        var pos = gridGen.GetCellPosition(gameCell);
                        CardsController.Instance.PlayCurrentCard(pos.x, pos.y);
                        TargetingEnabled = false;
                    }
                //}
            }

            if (cursor != null)
            {
                cursor.transform.position = gridGen.GetTopOfGridCell(targetCell);
            }

            // Plane p = `
            // if(Physics.Raycast(mouseRay.origin, mouseRay.direction))

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
            gridGen.GetAllCells().ForEach(c => { if (c is GameCell gc) { gc.isActionPreview = false; } });
        }

        public void ToggleOn()
        {
            TargetingEnabled = true;
        }
    }
}