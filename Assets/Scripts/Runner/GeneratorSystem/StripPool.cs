using System;
using System.Collections.Generic;
using Base.SimpleEventBus_and_MonoPool;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using Zenject;

namespace Runner.GeneratorSystem
{
    public class StripPool : IDisposable
    {
        private readonly Dictionary<int, ZenjectMonoBehaviourPool<PartStrip>> _defaultMonoBehaviourPools =
            new Dictionary<int, ZenjectMonoBehaviourPool<PartStrip>>();

        private readonly IDisposable _subscription;
        
        public StripPool(Settings settings, Transform parent, DiContainer diContainer)
        {
            _subscription = EventStreams.UserInterface.Subscribe<EventReleaseRunner>(Release);
            foreach (var partStrip in settings.PartStrips)
            {
                var pool = new ZenjectMonoBehaviourPool<PartStrip>(partStrip.PartStrip, parent, diContainer, partStrip.PoolSize);
                _defaultMonoBehaviourPools.Add(partStrip.PartStrip.ID, pool);
            }
        }

        public PartStrip GetPartStripByID(int id)
        {
            var partStrip = _defaultMonoBehaviourPools[id].Take();
            return partStrip;
        }

        private void Release(EventReleaseRunner eventReleaseRunner)
        {
            foreach (var defaultMonoBehaviourPool in _defaultMonoBehaviourPools)
            {
                defaultMonoBehaviourPool.Value.ReleaseAll();
            }
        }

        [Serializable]
        public class Settings
        {
            public List<PartStripPoolSettings> PartStrips = new List<PartStripPoolSettings>();
        }

        [Serializable]
        public class PartStripPoolSettings
        {
            public int PoolSize;
            public PartStrip PartStrip;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}