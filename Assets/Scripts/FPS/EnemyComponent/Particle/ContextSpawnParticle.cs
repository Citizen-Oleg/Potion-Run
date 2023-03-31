using ParticleFactory;
using UnityEngine;

namespace FPS.EnemyComponent
{
    public class ContextSpawnParticle : ParticleContext
    {
        public Color Color { get; }

        public ContextSpawnParticle(Color color)
        {
            Color = color;
        }
    }
}