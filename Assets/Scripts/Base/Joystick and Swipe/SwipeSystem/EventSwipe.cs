using SimpleEventBus.Events;

namespace Joystick_and_Swipe.SwipeSystem
{
    public class EventSwipe : EventBase
    {
        public SwipeType SwipeType { get; private set; }

        public void SetSwipeType(SwipeType swipeType)
        {
            SwipeType = swipeType;
        }
    }
}