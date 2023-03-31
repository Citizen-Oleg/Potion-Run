using System;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.UI;

namespace GapBetweenRunnerAndShooter.MixingSystem
{
    public class MixView : MonoBehaviour
    {
        [SerializeField]
        private Image _slider;

        private MixController _mixController;
        private IDisposable _subscription;

        private void Awake()
        {
            gameObject.SetActive(false);
            _subscription = EventStreams.UserInterface.Subscribe<EventStartMixingReagent>(Initialize);
        }

        private void Initialize(EventStartMixingReagent eventStartMixingReagent)
        {
            _mixController = eventStartMixingReagent.MixController;
            _slider.fillAmount = 0;
            gameObject.SetActive(true);
            _mixController.OnEndMixing += Hide;
        }

        private void Update()
        {
            if (_mixController != null)
            {
                _slider.fillAmount = _mixController.Progress;
            }
        }

        private void Hide(Color color)
        {
            _mixController.OnEndMixing -= Hide;
            _mixController = null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}