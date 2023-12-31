﻿using Assets.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Controls the grid.
    /// </summary>
    public class GridGenerator : MonoBehaviour
    {
        public float animationTime = 1.5f;
        
        /// <summary>
        /// Width of grid.
        /// </summary>
        public int width;

        /// <summary>
        /// Height of grid.
        /// </summary>
        public int height;

        /// <summary>
        /// Cell prefab.
        /// </summary>
        public GameObject cellPrefab;

        /// <summary>
        /// Cells matrix ([width, height]).
        /// </summary>
        protected Cell[,] cells;


        public Vector3 GridToWorldPosition(int x, int y)
        {
            var cell = cells[x, y];
            return cell.transform.position ;
        }

        public List<Vector3> GetAllCellPositions()
        {
            List<Vector3> poses = new List<Vector3>();
            foreach (var cell in cells)
            {
                // poses.Add(GetTopOfGridCell());
            }
            return poses;
        }


        public (int x, int y) GetCellPosition(Cell cell)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (cells[i, j].Equals(cell)) return (i, j);
            return (0, 0);
        }

        public Cell GetClosestCell(Ray raycastingRay)
        {
            if(cells == null || cells.Length == 0) return null;
            var tops = new List<Vector3>();
            foreach (var cell in cells)
            {
                var newTop = GetTopOfGridCell(cell);
                if (tops.Any(t => Vector3.Distance(newTop, t) < 0.01f)) continue ;
                tops.Add(newTop);
            }
            
            var allTops = new Dictionary<Cell, Vector3>();
            foreach (var cell in cells)
            {
                var newTop = GetTopOfGridCell(cell);
                allTops.Add(cell, newTop);
            }

            Cell candidate = null;
            float distance = 0;
            List<Vector3> candidates = new List<Vector3>();
            foreach (var top in tops)
            {
                Plane p = new Plane(Vector3.up, top);
                p.Raycast(raycastingRay, out var dist);
                var pos = raycastingRay.GetPoint(dist);
                candidates.Add(pos);
            }
            
            // Dictionary<C:ell, Vector3>

            var result = candidates.SelectMany(c => allTops.Select(t => (t.Key, Vector3.Distance(t.Value, c))))
                .OrderBy(can => can.Item2)
                .First();

            return result.Key;
        }
        
        public Vector3 GetTopOfGridCell(int x, int y)
        {
            var cell = cells[x, y];
            return cell.transform.position + new Vector3(0,cell.GetCompleteBounds().size.y,0);
        }
        
        public Vector3 GetTopOfGridCell(Cell cell)
        {
            return cell.transform.position + new Vector3(0,cell.GetCompleteBounds().size.y,0);
        }
        
        /// <summary>
        /// Generates the two-dimensional grid.
        /// </summary>
        protected void GenerateGrid(LevelGenerator levelGenerator)
        {
            if (width <= 0 || height <= 0)
            {
                Debug.LogError("Impossible grid dimensions!", gameObject);
                return;
            }

            // generate grid
            cells = new Cell[width, height];

            var scale = cellPrefab.transform.localScale;
            var origin = transform.position;
            var bottomLeft = new Vector3(
                origin.x - width * scale.x / 2f + scale.x / 2,
                origin.y,
                origin.z - height * scale.z / 2f + scale.z / 2
            );


            for (var x = 0; x < width; x++)
            for (var z = 0; z < height; z++)
            {
                var curPos = new Vector3(bottomLeft.x + x * scale.x, bottomLeft.y, bottomLeft.z + z * scale.z);

                // create new cell
                var cellObj = Instantiate(cellPrefab, curPos, Quaternion.identity, gameObject.transform);
                cellObj.name = $"({x}, {z})";
                cellObj.transform.localScale = new Vector3(1f, 0.01f, 1f);
                var cell = cellObj.GetComponent<Cell>();
                cell.levelGenerator = levelGenerator;
                cell.PopulateCell();
                cells[x, z] = cell;

                /*
                 * Assign neighbours
                 */

                if (x > 0)
                {
                    var leftCell = cells[x - 1, z];
                    cell.neighbours[3] = leftCell;
                    leftCell.neighbours[1] = cell;
                }

                if (z > 0)
                {
                    var bottomCell = cells[x, z - 1];
                    cell.neighbours[0] = bottomCell;
                    bottomCell.neighbours[2] = cell;
                }
            }

        }

        /// <summary>
        /// Destroys the current grid.
        /// </summary>
        protected async UniTask RemoveGrid()
        {
            List<UniTask> tasks = new List<UniTask>();
            float step = animationTime / gameObject.transform.childCount;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var c = gameObject.transform.GetChild(i);
                tasks.Add(c.DOScaleY(0.01f, 0.3f).SetDelay(step * i).AsyncWaitForCompletion().AsUniTask());
            }

            await UniTask.WhenAll(tasks);
        }

        internal Cell GetCell(int x, int y)
        {
            if (x >= cells.GetLength(0) || y >= cells.GetLength(1)) return null;
            if (cells == null) return null;
            if(x < 0 || y < 0) return null;
            return cells[x, y];
        }

        internal List<Cell> GetAllCells()
        {
            List<Cell> ret = new List<Cell>();
            if (cells == null) return ret;
            foreach (var item in cells)
            {
                ret.Add(item);
            }
            return ret;
        }
    }
}