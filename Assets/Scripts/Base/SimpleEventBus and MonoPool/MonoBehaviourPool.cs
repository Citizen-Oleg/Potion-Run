using System.Collections.Generic;
using System.Collections.ObjectModel;
using Runner.ReagentSystem;
using UnityEngine;

namespace Tools.SimpleEventBus
{
    public abstract class MonoBehaviourPool<T> where T : Component
    {
        public ReadOnlyCollection<T> UsedItems { get; private set; }

        protected readonly List<T> _notUsedItems = new List<T>();
        protected readonly List<T> _usedItems = new List<T>();

        protected readonly T _prefab;
        protected readonly Transform _parent;
        protected readonly Vector3 _defaultScale;

        public MonoBehaviourPool(T prefab, Transform parent, int defaultCount = 4)
        {
            _parent = parent;
            _prefab = prefab;
            _defaultScale = prefab.transform.localScale;
        }

        public void ResetParent(T item)
        {
            item.transform.parent = _parent;
        }

        protected void CreateItemInPool(int defaultCount)
        {
            for (int i = 0; i < defaultCount; i++)
            {
                AddNewItemInPool();
            }

            UsedItems = new ReadOnlyCollection<T>(_usedItems);
        }

        public T Take()
        {
            if (_notUsedItems.Count == 0)
            {
                AddNewItemInPool();
            }

            var lastIndex = _notUsedItems.Count - 1;
            var itemFromPool = _notUsedItems[lastIndex];
            _notUsedItems.RemoveAt(lastIndex);
            _usedItems.Add(itemFromPool);
            itemFromPool.gameObject.SetActive(true);

            return itemFromPool;
        }

        public void ReleaseAll(bool setDefaultParent = true)
        {
            for (int i = 0; i < _usedItems.Count; i++)
            {
                _usedItems[i].gameObject.SetActive(false);

                if (setDefaultParent)
                {
                    _usedItems[i].transform.parent = _parent;
                }
            }

            _notUsedItems.AddRange(_usedItems);
            _usedItems.Clear();
            
            SortBySiblingIndexUnused();
        }

        private void SortBySiblingIndexUnused()
        {
            _notUsedItems.Sort((a, b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex()));
        }

        public void Release(T item)
        {
            item.gameObject.SetActive(false);
            
            item.transform.parent = _parent;
            item.transform.localScale = _defaultScale;

            _usedItems.Remove(item);
            _notUsedItems.Add(item);
        }

        public void Release(List<T> items)
        {
            foreach (var item in items)
            {
                Release(item);
            }
        }

        protected virtual void AddNewItemInPool()
        {
            var newItem = Object.Instantiate(_prefab, _parent, false);
            newItem.gameObject.SetActive(false);
            _notUsedItems.Add(newItem);
        }
    }
}