using Level_selection.Map_system;
using SimpleEventBus.Events;

namespace Events
{
    public class EventTeleportPlayerToRegion : EventBase
    {
        public Region Region { get; }

        public EventTeleportPlayerToRegion(Region region)
        {
            Region = region;
        }
    }
}