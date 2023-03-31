using System;
using Events;
using SimpleEventBus.Disposables;
using Tools.SimpleEventBus;
using UnityEngine;

namespace FPS
{
    public class FailScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject _newRegion;
        
        private CompositeDisposable _subscription;
        
        private void Awake()
        {
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventShowFailScreen>(Show),
                EventStreams.UserInterface.Subscribe<EventReleaseRunner>(Hide),
                EventStreams.UserInterface.Subscribe<EventOpenNewRegion>(ShowPanelNewRegion)
            };
            
            gameObject.SetActive(false);
        }

        private void ShowPanelNewRegion(EventOpenNewRegion eventOpenNewRegion)
        {
            _newRegion.gameObject.SetActive(true);
        }
        
        private void Show(EventShowFailScreen eventShowCompletedScreen)
        {
            gameObject.SetActive(true);
        }

        private void Hide(EventReleaseRunner eventReleaseRunner)
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}