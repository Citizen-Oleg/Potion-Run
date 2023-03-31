using System;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Joystick_and_Swipe.SwipeSystem
{
    public class SwipeController : ITickable
    {
        private readonly EventSwipe _eventSwipe = new EventSwipe();
        private readonly float _deadZone;
        private readonly bool _isMobile;
        
        private Vector2 _tapPosition;
        private Vector2 _swipeDelta;

        private bool _isSwiping;

        public SwipeController(Settings settings)
        {
            _deadZone = settings.DeadZone;
            _isMobile = Application.isMobilePlatform;
        }

        public void Tick()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
#else
            if (Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
               return;
            }
#endif

            if (!_isMobile)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isSwiping = true;
                    _tapPosition = Input.mousePosition;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    CheckSwipe();
                    ResetSwipe();
                }
            }
            else if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    _isSwiping = true;
                    _tapPosition = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    CheckSwipe();
                    ResetSwipe();
                }
            }
        }

        private void CheckSwipe()
        {
            _swipeDelta = Vector2.zero;

            if (_isSwiping)
            {
                if (!_isMobile)
                {
                    _swipeDelta = (Vector2) Input.mousePosition - _tapPosition;
                }
                else
                {
                    _swipeDelta = Input.GetTouch(0).position - _tapPosition;
                }
            }

            if (_swipeDelta.magnitude > _deadZone)
            {
                if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y))
                {
                    _eventSwipe.SetSwipeType(_swipeDelta.x > 0 ? SwipeType.Right : SwipeType.Left);
                }
                else
                {
                    _eventSwipe.SetSwipeType(_swipeDelta.y > 0 ? SwipeType.Up : SwipeType.Down);
                }

                EventStreams.UserInterface.Publish(_eventSwipe);
            }
        }

        private void ResetSwipe()
        {
            _isSwiping = false;

            _tapPosition = Vector2.zero;
            _swipeDelta = Vector2.zero;
        }
    
        [Serializable]
        public struct Settings
        {
            public float DeadZone;
        }
    }
}