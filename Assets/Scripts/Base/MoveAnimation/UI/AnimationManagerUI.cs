using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Base.MoveAnimation.UI
{
    public class AnimationManagerUI : ITickable
    {
        private readonly float _travelTime;
        private readonly List<AnimationUIObject> _animationObjects = new List<AnimationUIObject>();

        private bool _isStop;

        public AnimationManagerUI(Settings settings)
        {
            _travelTime = settings.TravelTime;
        }

        public void ShowAnimationUI(RectTransform rectTransform, Vector2 startPosition, RectTransform endTransform, Action callBack)
        {
            _animationObjects.Add(new AnimationUIObject
            {
                UIObject = rectTransform,
                TravelTime = _travelTime,
                StartPosition = startPosition,
                EndTransform = endTransform,
                CallBack = callBack
            });
        }
        
        public void Tick()
        {
            if (_isStop)
            {
                return;
            }
            
            for (int i = 0; i < _animationObjects.Count; i++)
            {
                var information = _animationObjects[i];

                information.Progress += Time.deltaTime / information.TravelTime;

                var endTransform = information.EndTransform;
                var positionItem = Vector2.Lerp(information.StartPosition, endTransform.position, information.Progress);
                information.UIObject.position = positionItem;
                
                _animationObjects[i] = information;
                if (information.Progress >= 1)
                {
                    information.CallBack?.Invoke();
                    _animationObjects.RemoveAt(i);
                    i--;
                }
            }
        }

        public void StopAnimation()
        {
            _isStop = true;

            for (int i = 0; i < _animationObjects.Count; i++)
            {
                var information = _animationObjects[i];
                information.CallBack?.Invoke();
                _animationObjects.RemoveAt(i);
                i--;
            }

            _isStop = false;
        }

        [Serializable]
        public class Settings
        {
            public float TravelTime;
        }

        [Serializable]
        public struct AnimationUIObject
        {
            public float TravelTime;
            public RectTransform UIObject;
            public Vector2 StartPosition;
            public RectTransform EndTransform;
            public float Progress;
            public Action CallBack;
        }
    }
}