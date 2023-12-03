using Assets.Scripts.Frontend.Interaction;
using Assets.Scripts.Frontend.Interaction.UI;
using Cysharp.Threading.Tasks;
using Frontend.Interaction;
using GameState;
using GameState.PlayerCommand;
using LevelGeneration;
using System.Linq;
using Unity.Entities;
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
            Debug.Log("Updating Tiles");
            foreach (var tile in updateEvt.UpdatedTiles)
            {
                var cell = levelGen.GetCell(tile.X, tile.Y);
                if(cell is GameCell gc)
                {
                    gc.SetData(tile.card);
                    gc.Card = tile.card;
                    gc.isPlayerControlled = tile.PlayerInControl;
                }
            }
            Debug.Log("Tiles Updated.");
            await CardHand.Instance.ConfirmPlay();
            Debug.Log("Play Confirmed.");
            CardHand.Instance.UnselectAll();
            Targeting.Instance.ClearActionPlacement();
            Debug.Log("Tiles Unselected.");
        }

        public void EndTurn()
        {
            var processorSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameEventProcessorSystem>();
            processorSystem.Act(new EndTurnCommand());
        }

        public async UniTask HandleEndMapEvent(EndMapEvent endEvt)
        {
            if (AIController.Instance != null)
            {
                await CardHand.Instance.DiscardHand();
                await AIController.Instance.SetNewHP(endEvt.AIHealth);
                await CardHand.Instance.AttackBoss();
            }
        }

    }
}