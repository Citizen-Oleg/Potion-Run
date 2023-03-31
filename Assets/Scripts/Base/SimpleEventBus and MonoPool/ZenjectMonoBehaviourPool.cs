using Tools.SimpleEventBus;
using UnityEngine;
using Zenject;

namespace Base.SimpleEventBus_and_MonoPool
{
    public class ZenjectMonoBehaviourPool<T> : MonoBehaviourPool<T> where T : Component
    {
        private readonly DiContainer _diContainer;

        public ZenjectMonoBehaviourPool(T prefab, Transform parent, DiContainer diContainer, int defaultCount = 4) :
            base(prefab, parent, defaultCount)
        {
            _diContainer = diContainer;
            CreateItemInPool(defaultCount);
        }

        protected override void AddNewItemInPool()
        {
            var newItem = _diContainer.InstantiatePrefabForComponent<T>(_prefab, _parent);
            newItem.gameObject.SetActive(false);
            _notUsedItems.Add(newItem);
        }
    }
}