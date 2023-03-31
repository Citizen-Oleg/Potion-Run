using System.Collections.Generic;
using Runner;
using Runner.ReagentSystem;
using SimpleEventBus.Events;

namespace Events
{
    public class EventFinish : EventBase
    {
        public Boiler Boiler { get; }

        public EventFinish(Boiler boiler)
        {
            Boiler = boiler;
        }
    }
}