using System.Collections.Generic;
using ParticleFactory;
using UnityEngine;

namespace FPS.EnemyComponent
{
    public class SpawnParticle : TemporaryParticle
    {
        [SerializeField]
        private List<ParticleSystem> _changeColorParticle;

        public override void Initialize(ParticleContext particleContext)
        {
            var context = particleContext as ContextSpawnParticle;

            foreach (var system in _changeColorParticle)
            {
                var main = system.main;
                main.startColor = context.Color;
            }
        }
    }
}