using UnityEngine;

namespace ParticleFactory
{
    public class TemporaryParticle : BaseParticle
    {
        [SerializeField]
        private float _lifeTime;

        private float _endTime;

        public override void Activate()
        {
            _particleSystem.Clear();
            _particleSystem.Play();

            _endTime = Time.time + _lifeTime;
        }

        public override void Initialize(ParticleContext particleContext)
        {
        }

        protected virtual void Update()
        {
            if (_endTime <= Time.time)
            {
                ParticleSystemFactory.Release(this);
            }
        }
    }
}