using System;
using Events;
using GameAnalyticsSDK.Setup;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GapBetweenRunnerAndShooter.Screen
{
    public class OpenMapButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _isClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClick)
            {
                _isClick = true;
                EventStreams.UserInterface.Publish(new EventOpenMap());
            }
        }

        private void OnDisable()
        {
            _isClick = false;
        }
    }
}