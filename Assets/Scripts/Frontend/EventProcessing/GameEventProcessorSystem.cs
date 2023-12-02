using Assets.Scripts.Frontend.Interaction;
using Cysharp.Threading.Tasks;
using GameState;
using GameState.PlayerCommand;
using System;
using Unity.Entities;
using UnityEngine;

namespace Frontend.EventProcessing
{
    public partial class GameEventProcessorSystem : SystemBase
    {
        private GameStateMachine machine;

        private EventProcessingState currentlyProcessingEvent;

        private GridDebugger gDebugger;

        private bool isProcessing = false;

        private TimeTicker timeTicker;

        private MapEventProcessor mapProcessor;

        private TimeTickSuspender SuspendTick => new(timeTicker);
        
        public void BeginProcessing(GameStateMachine machine)
        {
            this.machine = machine;
            //timeTicker = new TimeTicker(this.machine);
            mapProcessor = GameObject.FindObjectOfType<MapEventProcessor>();
            machine.Start();
        }

        public void Act(IPlayerCommand command)
        {
            CoLogger.Log($"Send command: {command}");
            machine.Act((command));
        }

        public async UniTask Process()
        {
            if (machine == null || machine.State.Status == GameStatus.None || isProcessing) return;
            ProcessDebugInput();
            isProcessing = true;
            if(currentlyProcessingEvent == null || currentlyProcessingEvent.State == ProcessingState.Completed)
            {
                if( machine.EventQueue.Count > 0 )
                {
                    //CoLogger.Log("async processing");
                    var newArgs = machine.EventQueue.Dequeue();
                    currentlyProcessingEvent = new EventProcessingState(newArgs);
                    //CoLogger.Log($"processing event of type {newArgs.Event.GetType()} with id {newArgs.Id}");
                    if (newArgs.Event is GameStartedEvent gameStartedEvent)
                    {
                        CoLogger.Log($"gameStartedEvent {nameof(GameStartedEvent)}");
                        //thing.ProcessGameStartEvent(gameStartedEvent);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is GameEndedEvent gameEndedEvent)
                    {
                        CoLogger.Log($"gameEndedEvent{nameof(GameEndedEvent)}");
                        //thing.ProcessGameStartEvent(gameEndedEvent);
                        Debug.Log("Game Over");
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is LoadMapEvent loadMapEvent)
                    {
                        CoLogger.Log($"loadMapEvent {nameof(LoadMapEvent)}");
                        //thing.ProcessGameStartEvent(loadMapEvent);
                        await mapProcessor.HandleLevelEvent( loadMapEvent );
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is UpdateMapEvent updateMapEvent)
                    {
                        CoLogger.Log($"updateMapEvent {nameof(UpdateMapEvent)}");
                        await mapProcessor.HandleUpdateMapState( updateMapEvent );
                        //thing.ProcessGameStartEvent(updateMapEvent);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is EndMapEvent endMapEvent)
                    {
                        CoLogger.Log($"endMapEvent {nameof(EndMapEvent)}");
                        //thing.ProcessGameStartEvent(endMapEvent);
                        await mapProcessor.HandleEndMapEvent( endMapEvent );
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is DrawCardsEvent drawCardsEvent)
                    {
                        CoLogger.Log($"drawCardsEvent {nameof(DrawCardsEvent)}");
                        //thing.ProcessGameStartEvent(drawCardsEvent);
                        CardsController.Instance.HandleCardsEvent(drawCardsEvent);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is CardOptionsResponse cardOptionsResponse)
                    {
                        CoLogger.Log($"cardOptionsResponse {nameof(CardOptionsResponse)}");
                        //thing.ProcessGameStartEvent(cardOptionsResponse);
                        CardsController.Instance.HandleCardUpdateReponse(cardOptionsResponse);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is TurnUpdateEvent turnUpdateEvent)
                    {
                        CoLogger.Log($"turnUpdateEvent {nameof(TurnUpdateEvent)}");
                        //thing.ProcessGameStartEvent(cardOptionsResponse);
                        Debug.Log($"Turn # {turnUpdateEvent.Turn}");
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }

                    //CoLogger.Log("async processing finished");
                }
            }
            isProcessing = false;
        }
        
        public void ProcessDebugInput()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.LeftShift) && currentlyProcessingEvent != null)
            {
                currentlyProcessingEvent.SetState(ProcessingState.Completed);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKey(KeyCode.LeftShift) )
            {
                if(gDebugger != null) { return; }
                var go = new GameObject("gDebugger");
                this.gDebugger = go.AddComponent<GridDebugger>();
            }

            if (Input.GetKeyDown(KeyCode.Alpha8) && Input.GetKey(KeyCode.LeftShift))
            {
                machine.Start();
                //machine.Act();
            }
            #endif
        }

        protected override void OnUpdate()
        {
            //timeTicker.Update(SystemAPI.Time.ElapsedTime);
            Process();
        }

    }

    public static class GameColors
    {

        public static Color AIControlled = new Color(1, .75f, .2f, 1f);
        public static Color PlayerControlled = new Color(0.1f, 1f, .4f, 1f);
        public static Color Playable = new Color(0.2f, 1f, .8f, 1f);
        public static Color Highlighted = new Color(1f, 1f, 0.2f, 1f);

    }

    public class GridDebugger : MonoBehaviour
    {
        public int[][] grid;
        
        private void OnGUI()
        { 
            Vector2 start = new Vector2(10, 10);
            Vector2 stepX = new Vector2(30, 0);
            Vector2 stepY = new Vector2(0, 30);
            Vector2 size = new Vector2(20, 20);
            var offset= new Vector2(0, 0);
            foreach (var y in grid)
            {
                offset.x = 0;
                foreach (var x in y)
                {
                    GUI.Box(new Rect(start + offset , size), x.ToString());
                    offset += stepX;
                }
                offset += stepY;
            }
        }
    }

    public class TimeTicker
    {
        private readonly ITimeTickReceiver timeTickReceiver;

        public TimeTicker(ITimeTickReceiver timeTickReceiver)
        {
            this.timeTickReceiver = timeTickReceiver;
        }
        
        public long TimeForTick { get; set; } = 1;
        public bool Suspend { get; set; } = false;

        private long lastElapsed = 0;

        public void Update(double elapsedTime)
        {
            long newElapsedTime = (long)Math.Truncate(elapsedTime);
            while(newElapsedTime - lastElapsed >= TimeForTick)
            {
                lastElapsed += TimeForTick;

                if (Suspend is false)
                {
                    timeTickReceiver.Tick();
                }
            }
        }
    }

    public readonly struct TimeTickSuspender : IDisposable
    {
        private readonly TimeTicker timeTicker;

        public TimeTickSuspender(TimeTicker timeTicker)
        {
            this.timeTicker = timeTicker;
            timeTicker.Suspend = true;
        }
        public void Dispose()
        {
            timeTicker.Suspend = false;
        }
    }
}
