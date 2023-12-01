using GameState;
using System.Collections.ObjectModel;

namespace Tests.Editor
{
    public class DumGameStateEmitter : Collection<IGameEvent>, IGameEventEmitter
    {
        public void Emit(IGameEvent gameEvent)
        {
            Add(gameEvent);
        }

        public GameState.GameState State { get; set; }
    }
}