using SimpleEventBus;
using SimpleEventBus.Interfaces;

namespace Tools.SimpleEventBus
{
    public static class EventStreams
    {
        public static IEventBus UserInterface { get; } = new EventBus();
    }
}