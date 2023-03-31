using System;
using System.Collections.Generic;
using Base.SimpleEventBus_and_MonoPool;
using Events;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using Zenject;

namespace ParticleFactory
{
    public class ParticleSystemFactory : IDisposable
    {
        private Dictionary<ParticleType, ZenjectMonoBehaviourPool<BaseParticle>> _behaviourPools =
            new Dictionary<ParticleType, ZenjectMonoBehaviourPool<BaseParticle>>();
        private readonly CompositeDisposable _subscription;
        private readonly ParticleContext _context = new ParticleContext();

        public ParticleSystemFactory(Settings settings, Transform parent, DiContainer diContainer)
        {
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventShowParticle>(Create),
                EventStreams.UserInterface.Subscribe<EventShowContextParticle>(Create)
            };
            
            foreach (var settingsParticle in settings.Particles)
            {
                var pool = new ZenjectMonoBehaviourPool<BaseParticle>(settingsParticle, parent.transform, diContainer,settings.PoolSize);
                _behaviourPools.Add(settingsParticle.ParticleType, pool);
            }
        }
        
        private void Create(EventShowParticle eventShowParticle)
        {
            Create(eventShowParticle.Transform, eventShowParticle.ParticleType, _context);
        }
        
        private void Create(EventShowContextParticle eventShowParticle)
        {
            Create(eventShowParticle.Transform, eventShowParticle.ParticleType, eventShowParticle.ParticleContext);
        }

        private void Create<TContext>(Transform transform, ParticleType particleType, TContext context)
            where TContext : ParticleContext
        {
            var particle = _behaviourPools[particleType].Take();
            
            particle.Initialize(context);
            particle.transform.position = transform.position;
            particle.ParticleSystemFactory = this;
            particle.Activate();
        }

        public void Release(BaseParticle particle)
        {
            _behaviourPools[particle.ParticleType].Release(particle);
        }

        [Serializable]
        public class Settings
        {
            public int PoolSize;
            public List<BaseParticle> Particles = new List<BaseParticle>();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}