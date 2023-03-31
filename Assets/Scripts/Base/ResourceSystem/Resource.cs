using System;

namespace ResourceSystem
{
    [Serializable]
    public struct Resource
    {
        public ResourceType ResourceType;
        public int Amount;
    }
}