using FPS.EnemyComponent;
using SimpleEventBus.Events;

namespace Events
{
    public class EventEnemyDead : EventBase 
    {
        public Enemy Enemy { get; }

        public EventEnemyDead(Enemy enemy)
        {
            Enemy = enemy;
        }
    }
}