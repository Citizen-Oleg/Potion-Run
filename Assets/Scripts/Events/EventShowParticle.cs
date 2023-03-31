using ParticleFactory;
using SimpleEventBus.Events;
using UnityEngine;

namespace Events
{
    public class EventShowParticle : EventBase
    {
        public Transform Transform { get; set; }
        public ParticleType ParticleType { get; set; }
    }
}