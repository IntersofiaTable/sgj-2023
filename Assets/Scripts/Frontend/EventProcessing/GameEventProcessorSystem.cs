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

        private TimeTickSuspender SuspendTick => new(timeTicker);
        
        public void BeginProcessing(GameStateMachine machine)
        {
            this.machine = machine;
            //timeTicker = new TimeTicker(this.machine);
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
                    CoLogger.Log("async processing");
                    var newArgs = machine.EventQueue.Dequeue();
                    currentlyProcessingEvent = new EventProcessingState(newArgs);
                    CoLogger.Log($"processing event of type {newArgs.Event.GetType()} with id {newArgs.Id}");
                    if (newArgs.Event is GameStartedEvent gameStartedEvent)
                    {
                        CoLogger.Log($"do nothing {nameof(GameStartedEvent)}");
                        //thing.ProcessGameStartEvent(gameStartedEvent);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is LoadMapEvent loadMapEvent)
                    {
                        CoLogger.Log($"loadMapEvent {nameof(LoadMapEvent)}");
                        //thing.ProcessGameStartEvent(loadMapEvent);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is DrawCardsEvent drawCardsEvent)
                    {
                        CoLogger.Log($"drawCardsEvent {nameof(DrawCardsEvent)}");
                        //thing.ProcessGameStartEvent(drawCardsEvent);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    if (newArgs.Event is CardOptionsResponse cardOptionsResponse)
                    {
                        CoLogger.Log($"cardOptionsResponse {nameof(CardOptionsResponse)}");
                        //thing.ProcessGameStartEvent(cardOptionsResponse);
                        currentlyProcessingEvent.SetState(ProcessingState.Completed);
                    }
                    CoLogger.Log("async processing finished");
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
