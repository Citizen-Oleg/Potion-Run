using Tools.SimpleEventBus;
using UnityEngine;

namespace Base.SimpleEventBus_and_MonoPool
{
    public class DefaultMonoBehaviourPool<T> : MonoBehaviourPool<T> where T : Component 
    {
        public DefaultMonoBehaviourPool(T prefab, Transform parent, int defaultCount = 4) : base(prefab, parent, defaultCount)
        {
            CreateItemInPool(defaultCount);
        }
    }
}