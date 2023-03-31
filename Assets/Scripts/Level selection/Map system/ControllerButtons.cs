using System;
using Events;
using Level_selection.Bestiary_system;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;

namespace Level_selection.Map_system
{
    public class ControllerButtons : MonoBehaviour
    {
        [SerializeField]
        private BestiaryButton _bestiaryButton;
        [SerializeField]
        private PlayButton _playButton;

        private CompositeDisposable _subscription;
        
        private void Awake()
        {
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventHideMapButtons>(HideButtons),
                EventStreams.UserInterface.Subscribe<EventOpenMapButtons>(OpenButtons)
            };
            
        }

        private void HideButtons(EventHideMapButtons eventHideMapButtons)
        {
            _bestiaryButton.gameObject.SetActive(false);
            _playButton.gameObject.SetActive(false);
        }

        private void OpenButtons(EventOpenMapButtons eventOpenMapButtons)
        {
            _bestiaryButton.gameObject.SetActive(true);
            _playButton.gameObject.SetActive(true);
        }
        

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}