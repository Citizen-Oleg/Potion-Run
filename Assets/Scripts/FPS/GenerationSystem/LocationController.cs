using System;
using Dreamteck.Splines;
using Events;
using JetBrains.Annotations;
using Tools.SimpleEventBus;
using UnityEngine;

namespace FPS.GenerationSystem
{
    public class LocationController : MonoBehaviour
    {
        [SerializeField]
        private FPSPlayer _fpsPlayer;
        [SerializeField]
        private float _defaultSpeed;
        [SerializeField]
        private SplineFollower _splineFollower;
        [SerializeField]
        private SplineComputer _splineComputer;

        private StripPoint _currentStripPoint;
        private IDisposable _subscription;

        private void OnEnable()
        {
            _splineComputer.RebuildImmediate(true, true);
            _splineFollower.followSpeed = _defaultSpeed;
            _subscription = EventStreams.UserInterface.Subscribe<EventStartFPS>(StartFPS);
        }

        private void OnDisable()
        {
            _splineFollower.followSpeed = 0;
            _splineFollower.SetDistance(0);
            _splineComputer.gameObject.SetActive(false);
            _subscription?.Dispose();    
        }

        private void StartFPS(EventStartFPS eventStartFps)
        {
            SetDefaultSpeed();
        }

        [UsedImplicitly]
        public void SetDefaultSpeed()
        {
            if (_fpsPlayer.IsDead)
            {
                return;
            }
            
            _splineFollower.followSpeed = _defaultSpeed;
        }

        [UsedImplicitly]
        public void ChangeSpeed(float speed)
        {
            _splineFollower.followSpeed = speed;
        }

        [UsedImplicitly]
        public void ReachedPointEnemies(StripPoint stripPoint)
        {
            if (stripPoint.IsCleared)
            {
                return;
            }

            StopFollower();
            _currentStripPoint = stripPoint;
            _currentStripPoint.OnClear += ResumeMovement;
        }
        
        private void StopFollower()
        {
            _splineFollower.followSpeed = 0;
        }

        private void ResumeMovement()
        {
            _currentStripPoint.OnClear -= ResumeMovement;
            SetDefaultSpeed();
        }
    }
}