using ParticleFactory;

namespace FPS.EnemyComponent
{
    public class MonsterHitParticle : BaseParticle
    {
        public override void Activate()
        {
            _particleSystem.Clear();
            _particleSystem.Play();
        }

        public override void Initialize(ParticleContext particleContext)
        {
        }
    }
}