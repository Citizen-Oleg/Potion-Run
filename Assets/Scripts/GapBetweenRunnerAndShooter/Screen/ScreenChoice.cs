using System;
using DG.Tweening;
using Events;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;

namespace GapBetweenRunnerAndShooter.ScreenChoice
{
    public class ScreenChoice : MonoBehaviour
    {
        private CompositeDisposable _subscription;

        private void Awake()
        {
            gameObject.SetActive(false);
       
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventShowScreenChoice>(Show),
                EventStreams.UserInterface.Subscribe<EventHideScreenChoice>(Hide)
            };
        }

        private void Show(EventShowScreenChoice eventShowScreenChoice)
        {
            gameObject.SetActive(true);
        }

        private void Hide(EventHideScreenChoice eventHideScreenChoice)
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}