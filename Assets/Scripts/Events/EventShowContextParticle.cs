using ParticleFactory;
using SimpleEventBus.Events;
using UnityEngine;

namespace Events
{
    public class EventShowContextParticle : EventBase
    {
        public ParticleContext ParticleContext { get; } 
        public Transform Transform { get; }
        public ParticleType ParticleType { get; }

        public EventShowContextParticle(Transform transform, ParticleType particleType, ParticleContext particleContext)
        {
            ParticleContext = particleContext;
            Transform = transform;
            ParticleType = particleType;
        }
    }
}