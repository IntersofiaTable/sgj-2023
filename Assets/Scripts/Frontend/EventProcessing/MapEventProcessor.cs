using Cysharp.Threading.Tasks;
using GameState;
using LevelGeneration;
using UnityEngine;

namespace Frontend.EventProcessing
{
    public class MapEventProcessor : MonoBehaviour
    {
        public LevelGenerator levelGen;

        public async UniTask HandleLevelEvent(LoadMapEvent mapEvt)
        {
            levelGen.width = mapEvt.MapState.X;
            levelGen.height = mapEvt.MapState.Y;
            levelGen.GenerateLevel();
        }
    }
}