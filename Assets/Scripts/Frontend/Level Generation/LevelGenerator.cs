using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace LevelGeneration
{
    /// <summary>
    /// Generates level using the wave-function-collapse algorithm.
    /// </summary>
    public class LevelGenerator : GridGenerator
    {
        /// <summary>
        /// The modules.
        /// </summary>
        public List<Module> modules;

        /// <summary>
        /// The start module.
        /// </summary>
        public Module startModule;

        /// <summary>
        /// The goal module.
        /// </summary>
        public Module goalModule;

        /// <summary>
        /// Stores the cells in a heap having the closest cell to being solved as first element.
        /// </summary>
        public Heap<Cell> orderedCells;

        /// <summary>
        /// RNG seed.
        ///
        /// If set to -1 a random seed will be selected for every level generation.
        /// </summary>
        [Tooltip("If set to -1 a random seed will be selected for every level generation.")]
        public int seed;

        //private void Start()
        //{
        //    GenerateLevel();
        //}

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.G))
        //    {
        //        GenerateLevel();
        //    }
        //}

        /// <summary>
        /// Wave-function-collapse algorithm.
        /// </summary>
        public async UniTask GenerateLevel()
        {
            Debug.Log("removing grid.");
            await RemoveGrid();
            Debug.Log("grid removed.");

            GenerateGrid(this);

            var finalSeed = seed != -1 ? seed : Environment.TickCount;

            Random.InitState(finalSeed);

            // instantiate cells heap
            orderedCells = new Heap<Cell>(cells.GetLength(0) * cells.GetLength(1));

            for (var i = 0; i < cells.GetLength(0); i++)
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                orderedCells.Add(cells[i, j]);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // make sure the level fits the initial constraints
            ApplyInitialConstraints();

            // wave-function-collapse algorithm
            while (orderedCells.Count > 0)
            {
                // get cell that is closest to being solved (can also already be solved)
                var cell = orderedCells.GetFirst();

                if (cell.possibleModules.Count == 1)
                {
                    // cell is already solved -> remove finished cell from heap
                    cell.isFinal = true;
                    orderedCells.RemoveFirst();
                }
                else
                {
                    // set a random module for this cell
                    cell.SetModule(cell.possibleModules[Random.Range(0, cell.possibleModules.Count)]);
                }
            }

            stopwatch.Stop();
            Debug.Log(
                $"Wave-function-collapse algorithm finished in {stopwatch.Elapsed.TotalMilliseconds}ms (Seed: {finalSeed})");

            // instantiate module game objects
            
            List<UniTask> tasks = new List<UniTask>();
            int cellcnt = 0;
            foreach (var cell in cells)
            {
                var t = cell.transform;
                var delay = animationTime / (height * width) * cellcnt;
                Instantiate(cell.possibleModules[0].moduleGO, t.position, Quaternion.identity, t);
                tasks.Add(t.transform.DOScaleY(1, 0.3f).SetDelay(delay).AsyncWaitForCompletion().AsUniTask());
                cellcnt++;
            }
            await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// Resolve all initial constraints.
        /// </summary>
        private void ApplyInitialConstraints()
        {
            StartGoalConstraint();
            // BorderOutsideConstraint();
        }

        /// <summary>
        /// Initial constraint: There can only be border on the outside.
        /// </summary>
        private void BorderOutsideConstraint()
        {
            var bottomFilter = new EdgeFilter(2, Module.EdgeConnectionTypes.Block, true);
            var topFilter = new EdgeFilter(0, Module.EdgeConnectionTypes.Block, true);
            var leftFilter = new EdgeFilter(1, Module.EdgeConnectionTypes.Block, true);
            var rightFilter = new EdgeFilter(3, Module.EdgeConnectionTypes.Block, true);

            // filter bottom and top cells for only border
            for (var i = 0; i < 2; i++)
            {
                var z = i * (height - 1);

                for (var x = 0; x < width; x++)
                {
                    cells[x, z].FilterCell(i == 0 ? bottomFilter : topFilter);
                }
            }

            // filter left and right cells for only border
            for (var i = 0; i < 2; i++)
            {
                var x = i * (width - 1);

                for (var z = 0; z < height; z++)
                {
                    cells[x, z].FilterCell(i == 0 ? leftFilter : rightFilter);
                }
            }
        }

        /// <summary>
        /// Initial constraint: Place one start and one goal module.
        /// </summary>
        private void StartGoalConstraint()
        {
            var startCell = cells[Random.Range(0, cells.GetLength(0)), Random.Range(0, cells.GetLength(1) - 1)];
            Cell goalCell;

            startCell.SetModule(startModule);

            do
            {
                goalCell = cells[Random.Range(0, cells.GetLength(0)), Random.Range(1, cells.GetLength(1))];
            } while (goalCell == startCell);

            goalCell.SetModule(goalModule);
        }
    }
}