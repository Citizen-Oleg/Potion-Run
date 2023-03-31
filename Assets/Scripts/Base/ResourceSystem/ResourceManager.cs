using System;
using System.Collections.Generic;
using System.Linq;

namespace ResourceSystem
{
    public abstract class ResourceManager
    {
        public event Action<Resource> OnResourceChange;
        public event Action<int> OnPayResource;

        public List<Resource> Resources { get; protected set; }
        
        public ResourceManager(Settings settings)
        {
            Resources = new List<Resource>(settings.StartResource);
        }

        public virtual void AddResource(Resource resource)
        {
            AddResource(resource.ResourceType, resource.Amount);
        }
        
        public void AddResource(List<Resource> resources)
        {
            foreach (var resource in resources)
            {
                AddResource(resource);
            }
        }
        
        public virtual void AddResource(ResourceType resourceType, int amount)
        {
            var index = GetIndexResource(resourceType);
            var resource = Resources[index];

            resource.Amount += amount;
            Resources[index] = resource;
            
            OnResourceChange?.Invoke(Resources[index]);
        }
        
        public void Pay(ResourceType resourceType, int amount)
        {
            var index = GetIndexResource(resourceType);
            var resource = Resources[index];

            resource.Amount -= amount;
            Resources[index] = resource;
                
            OnResourceChange?.Invoke(Resources[index]);
            OnPayResource?.Invoke(amount);
        }

        public void Pay(Resource resource)
        {
            Pay(resource.ResourceType, resource.Amount);
        }

        public bool HasEnough(ResourceType type, int amount)
        {
            return Resources[GetIndexResource(type)].Amount >= amount;
        }

        public bool HasEnough(Resource resource)
        {
            return HasEnough(resource.ResourceType, resource.Amount);
        }

        public Resource GetResource(ResourceType resourceType)
        {
            return Resources.FirstOrDefault(resource => resource.ResourceType == resourceType);
        }

        private int GetIndexResource(ResourceType resourceType)
        {
            return Resources.FindIndex(resource => resource.ResourceType == resourceType);
        }

        [Serializable]
        public class Settings
        {
            public List<Resource> StartResource = new List<Resource>();
        }
    }
}
