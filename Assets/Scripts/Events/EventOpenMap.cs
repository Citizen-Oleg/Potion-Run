using SimpleEventBus.Events;

namespace Events
{
    public class EventOpenMap : EventBase
    {
        public bool IsOpenGame { get; }

        public EventOpenMap(bool isOpenGame = false)
        {
            IsOpenGame = isOpenGame;
        }
    }
}