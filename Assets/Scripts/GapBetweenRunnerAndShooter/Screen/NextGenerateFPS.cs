using System;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GapBetweenRunnerAndShooter.ScreenChoice
{
    public class NextGenerateFPS : MonoBehaviour, IPointerClickHandler
    {
        private bool _isClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClick)
            {
                _isClick = true;
                EventStreams.UserInterface.Publish(new EventGenerateShooter());
                EventStreams.UserInterface.Publish(new EventHideScreenChoice());
            }
        }

        private void OnDisable()
        {
            _isClick = false;
        }
    }
}