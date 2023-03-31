using System;
using Events;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level_selection.Bestiary_system
{
    public class BestiaryButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject _panelNewMonster;

        private CompositeDisposable _subscription;

        private void Awake()
        {
            _subscription = new CompositeDisposable()
            {
                EventStreams.UserInterface.Subscribe<EventOpenNewModel>(newModel => _panelNewMonster.SetActive(true)),
                EventStreams.UserInterface.Subscribe<EventOpenBestiary>(HideNewPanel)
            };
            
        }

        private void HideNewPanel(EventOpenBestiary eventOpenBestiary)
        {
            _panelNewMonster.SetActive(false);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            EventStreams.UserInterface.Publish(new EventOpenBestiary());
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}