using System;
using System.Collections.Generic;
using Assets.Scripts.Managers.ScreensManager;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LoadingSystem
{
    public class ScreenLoader
    {
        private readonly Dictionary<ScreenType, GameObject> _cancedObjects = new Dictionary<ScreenType, GameObject>();

        public async UniTask<T> ScreenLoadInternal<T>(ScreenType screenType, string assetID)
        {
            var screen = await Addressables.InstantiateAsync(assetID);

            if (!screen.TryGetComponent(out T component))
            {
                throw new NullReferenceException(
                    $"Object type {typeof(T)} is null on attempt to load it from addressables");
            }
         
            if (_cancedObjects.ContainsKey(screenType))
            {
                UnloadInternalByScreenType(screenType);
            }
            
            _cancedObjects.Add(screenType, screen);
            return component;
        }

        public void UnloadInternalByScreenType(ScreenType screenType)
        {
            if (_cancedObjects.TryGetValue(screenType, out GameObject cachedObject))
            {
                cachedObject.SetActive(false);
                _cancedObjects.Remove(screenType);
                Addressables.ReleaseInstance(cachedObject);
            }
        }
    }
}