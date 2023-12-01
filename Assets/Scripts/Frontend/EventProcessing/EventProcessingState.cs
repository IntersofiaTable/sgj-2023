using GameState;
using System;

namespace Frontend.EventProcessing
{
    public partial class GameEventProcessorSystem
    {
        public class EventProcessingState
        {

            public ProcessingState State;

            public GameEventArgs EventData;

            public EventProcessingState(GameEventArgs data)
            {
                State = ProcessingState.Waiting;
                EventData = data;
            }

            public void SetState(ProcessingState newState)
            {
                if(newState != State)
                {
                    State = newState;
                }
                else
                {
                    throw new Exception("What the fuck are you doing my nigger?");
                }
            }
        }
    }
}
