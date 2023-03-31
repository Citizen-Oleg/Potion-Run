using GapBetweenRunnerAndShooter.MixingSystem;
using SimpleEventBus.Events;

namespace Events
{
    public class EventStartMixingReagent : EventBase
    {
        public MixController MixController { get; }

        public EventStartMixingReagent(MixController mixController)
        {
            MixController = mixController;
        }
    }
}