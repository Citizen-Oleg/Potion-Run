using System;
using Events;
using JetBrains.Annotations;
using Tools.SimpleEventBus;
using UnityEngine;

namespace GapBetweenRunnerAndShooter.Screen
{
    public class NewMonsterView : MonoBehaviour
    {
        private IDisposable _subscription;

        private void Awake()
        {
            _subscription = EventStreams.UserInterface.Subscribe<EventOpenNewModel>(Show);
            gameObject.SetActive(false);
        }

        private void Show(EventOpenNewModel eventOpenNewModel)
        {
            gameObject.SetActive(true);
        }

        [UsedImplicitly]
        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}