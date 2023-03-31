using System;
using Events;
using GameAnalyticsSDK.Setup;
using SimpleEventBus.Disposables;
using TMPro;
using Tools.SimpleEventBus;
using UnityEngine;

namespace FPS
{
    public class CompletedScreen : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textReagent;
        [SerializeField]
        private GameObject _newRegion;
        [SerializeField]
        private GameObject _newReagentPanel;
        
        private CompositeDisposable _subscription;
        
        private void Awake()
        {
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventShowCompletedScreen>(Show),
                EventStreams.UserInterface.Subscribe<EventReleaseRunner>(Hide),
                EventStreams.UserInterface.Subscribe<EventOpenNewRegion>(ShowPanelNewRegion),
                EventStreams.UserInterface.Subscribe<EventShowPanelNewReagent>(ShowPanelNewReagent),
                EventStreams.UserInterface.Subscribe<EventHidePanelNewReagent>(HidePanelNewReagent)
                
            };

            gameObject.SetActive(false);
        }

        private void ShowPanelNewReagent(EventShowPanelNewReagent eventShowPanelNewReagent)
        {
            _newReagentPanel.SetActive(true);
        }

        private void HidePanelNewReagent(EventHidePanelNewReagent eventHidePanelNewReagent)
        {
            _newReagentPanel.SetActive(false);
        }
        
        private void ShowPanelNewRegion(EventOpenNewRegion eventOpenNewRegion)
        {
            _newRegion.gameObject.SetActive(true);
        }

        private void Show(EventShowCompletedScreen eventShowCompletedScreen)
        {
            _textReagent.text = eventShowCompletedScreen.ReagentName;
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