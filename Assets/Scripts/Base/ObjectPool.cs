using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Base
{
    public class ObjectPool
    {
        public ReadOnlyCollection<GameObject> UsedItems { get; private set; }

        private readonly List<GameObject> _notUsedItems = new List<GameObject>();
        private readonly List<GameObject> _usedItems = new List<GameObject>();

        private readonly GameObject _prefab;
        private readonly Transform _parent;

        public ObjectPool(GameObject prefab, Transform parent, int defaultCount = 4)
        {
            _parent = parent;
            _prefab = prefab;

            for (int i = 0; i < defaultCount; i++)
            {
                AddNewItemInPool();
            }

            UsedItems = new ReadOnlyCollection<GameObject>(_usedItems);
        }

        public GameObject Take()
        {
            if (_notUsedItems.Count == 0)
            {
                AddNewItemInPool();
            }

            var lastIndex = _notUsedItems.Count - 1;
            var itemFromPool = _notUsedItems[lastIndex];
            _notUsedItems.RemoveAt(lastIndex);
            _usedItems.Add(itemFromPool);

            itemFromPool.gameObject.transform.localScale = Vector3.one;
            itemFromPool.gameObject.SetActive(true);

            return itemFromPool;
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < _usedItems.Count; i++)
            {
                _usedItems[i].gameObject.SetActive(false);
            }

            _notUsedItems.AddRange(_usedItems);
            _usedItems.Clear();
            
            SortBySiblingIndexUnused();
        }

        private void SortBySiblingIndexUnused()
        {
            _notUsedItems.Sort((a, b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex()));
        }

        public void Release(GameObject item)
        {
            item.gameObject.SetActive(false);

            _usedItems.Remove(item);
            _notUsedItems.Add(item);
        }

        public void Release(List<GameObject> items)
        {
            foreach (var item in items)
            {
                Release(item);
            }
        }

        private void AddNewItemInPool()
        {
            var newItem = Object.Instantiate(_prefab, _parent, false);
            newItem.gameObject.SetActive(false);
            _notUsedItems.Add(newItem);
        }
    }
}