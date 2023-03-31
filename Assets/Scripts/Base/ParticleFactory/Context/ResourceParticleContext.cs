using ResourceSystem;

namespace ParticleFactory.Context
{
    public class ResourceParticleContext : ParticleContext
    {
        public Resource Resource { get; }

        public ResourceParticleContext(Resource resource)
        {
            Resource = resource;
        }
    }
}