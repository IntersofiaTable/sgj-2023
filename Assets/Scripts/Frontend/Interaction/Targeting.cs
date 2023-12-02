using System;
using LevelGeneration;
using UnityEngine;

namespace Frontend.Interaction
{
    public class Targeting : MonoBehaviour
    {
        private GridGenerator gridGen;

        public GameObject cursor;
        
        private void Start()
        {
            gridGen = FindObjectOfType<GridGenerator>();
        }

        public void Update()
        {
            GetMouseInput();
        }


        public void GetMouseInput()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            var targetCell = gridGen.GetClosestCell(mouseRay);

            if (cursor != null)
            {
                cursor.transform.position = gridGen.GetTopOfGridCell(targetCell);
            }
            // Plane p = `
            // if(Physics.Raycast(mouseRay.origin, mouseRay.direction))

        }
    }
}