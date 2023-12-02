using Assets.Scripts.Frontend.Interaction;
using Cysharp.Threading.Tasks;
using Frontend.Interaction;
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
            Debug.Log("Generating Level.");
            await levelGen.GenerateLevel();
            Debug.Log("Level Generated");
        }
        

        public async UniTask HandleUpdateMapState(UpdateMapEvent updateEvt)
        {
            foreach (var tile in updateEvt.UpdatedTiles)
            {
                var cell = levelGen.GetCell(tile.X, tile.Y);
                if(cell is GameCell gc)
                {
                    gc.Card = tile.card;
                    gc.isPlayerControlled = tile.PlayerInControl;
                }
            }
        }

        public async UniTask HandleEndMapEvent(EndMapEvent endEvt)
        {
            if (AIController.Instance != null)
            {
                await AIController.Instance.SetNewHP(endEvt.AIHealth);
            }
        }

    }
}