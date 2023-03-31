using Events;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level_selection.Map_system
{
    public class PlayButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _isClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClick)
            {
                _isClick = true;
                EventStreams.UserInterface.Publish(new EventPlay());
            }
        }
        
        private void OnDisable()
        {
            _isClick = false;
        }
    }
}