using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GapBetweenRunnerAndShooter.Screen
{
    public class ContinueButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _isClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClick)
            {
                _isClick = true;
                EventStreams.UserInterface.Publish(new EventContinue());
            }
        }

        private void OnDisable()
        {
            _isClick = false;
        }
    }
}