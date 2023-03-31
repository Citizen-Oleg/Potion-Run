using System.Collections;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GapBetweenRunnerAndShooter.ScreenChoice
{
    public class RestartButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _isClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClick)
            {
                _isClick = true;
               EventStreams.UserInterface.Publish(new EventRestart());
            }
        }
        private void OnDisable()
        {
            _isClick = false;
        }
    }
}