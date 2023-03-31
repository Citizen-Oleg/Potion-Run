using FPS.GenerationSystem;
using SimpleEventBus.Events;

namespace Events
{
    public class EventCreateLocation : EventBase
    {
        public Location Location { get; }

        public EventCreateLocation(Location location)
        {
            Location = location;
        }
    }
}