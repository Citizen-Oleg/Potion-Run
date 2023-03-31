using System;
using System.Collections.Generic;
using Base.MoveAnimation._3D;
using Events;
using Runner.GeneratorSystem;
using Runner.ReagentSystem;
using Tools.SimpleEventBus;
using UnityEngine;
using Zenject;

namespace Runner
{
    public class Start : PartStrip
    {
        [SerializeField]
        private Boiler _boiler;
        [SerializeField]
        private List<Transform> _saveStartPositions = new List<Transform>();

        private List<InfoPosition> _infoPositions;
        private IDisposable _subscription;

        private void Awake()
        {
            _subscription = EventStreams.UserInterface.Subscribe<EventReleaseRunner>(SetDefaultPosition);
            _infoPositions = new List<InfoPosition>(_saveStartPositions.Count);
            foreach (var saveStartPosition in _saveStartPositions)
            {
                var infoPosition = new InfoPosition
                {
                    Position = saveStartPosition.position,
                    Rotation = saveStartPosition.rotation,
                    Transform = saveStartPosition
                };
                
                _infoPositions.Add(infoPosition);
            }
        }

        [Inject]
        public void Constructor(ReagentModelProvider reagentModelProvider, AnimationManager animationManager, DynamicJoystick variableJoystick)
        {
            _boiler.Initialize(reagentModelProvider, animationManager, variableJoystick);
        }

        private void SetDefaultPosition(EventReleaseRunner eventReleaseRunner)
        {
            foreach (var infoPosition in _infoPositions)
            {
                infoPosition.Transform.position = infoPosition.Position;
                infoPosition.Transform.rotation = infoPosition.Rotation;
            }
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
        
        public struct InfoPosition
        {
            public Transform Transform;
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}