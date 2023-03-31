using System;
using System.Runtime.CompilerServices;
using Base.MoveAnimation._3D;
using DG.Tweening;
using Dreamteck.Splines;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using Zenject;

namespace FPS
{
    public class FPSPlayer : MonoBehaviour
    {
        public bool IsDead => _isDead;

        [SerializeField]
        private Transform _hands;

        [SerializeField]
        private float _timeTravel;
        [SerializeField]
        private Transform _startPoint;
        [SerializeField]
        private Transform _endPoint;

        [Inject]
        private AnimationManager _animationManager;
        
        private bool _isDead;

        private void OnEnable()
        {
            _hands.transform.position = _startPoint.position;
            _animationManager.ShowMoveObject(_hands, _endPoint, Vector3.zero, Vector3.zero,
                () => { }, false, false, _timeTravel, CurveType.Line);
    }

        private void OnDisable()
        {
            _isDead = false;
        }

        public void Dead()
        {
            if (_isDead)
            {
                return;
            }

            _isDead = true;
            EventStreams.UserInterface.Publish(new EventShowFailScreen());
        }
        
        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 5f, transform.position.z));
        }
    }
}