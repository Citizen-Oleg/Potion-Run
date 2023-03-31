using UnityEngine;

namespace ParticleFactory
{
    public abstract class BaseParticle : MonoBehaviour
    {
        public ParticleSystemFactory ParticleSystemFactory { protected get; set; }
        public ParticleType ParticleType => _typeParticle;
        
        [SerializeField]
        protected ParticleType _typeParticle;
        [SerializeField]
        protected ParticleSystem _particleSystem;

        public abstract void Activate();
        public abstract void Initialize(ParticleContext particleContext);
    }
}