using Runner.ReagentSystem;
using SimpleEventBus.Events;

namespace Events
{
    public class EventShowCompletedScreen : EventBase
    {
        public string ReagentName { get;}

        public EventShowCompletedScreen(string reagentName)
        {
            ReagentName = reagentName;
        }
    }
}