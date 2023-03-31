﻿using System;
using CameraComponent;
using DG.Tweening;
using Dreamteck.Splines;
using Events;
using FPS.EnemyComponent;
using FPS.GenerationSystem;
using GapBetweenRunnerAndShooter.CraftSystem;
using GapBetweenRunnerAndShooter.MixingSystem;
using JetBrains.Annotations;
using Runner;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using Zenject;

namespace GapBetweenRunnerAndShooter
{
    public class Laboratory : MonoBehaviour
    {
        [SerializeField]
        private float _timeMoveCamera;
        [SerializeField]
        private float _speedCameraFollower;
        [SerializeField]
        private Transform _mixingChamberCameraPosition;
        [SerializeField]
        private float _timeMoveBoiler;
        [SerializeField]
        private Transform _boilerPosition;
        [SerializeField]
        private SplineComputer _splineComputerCamera;
        [SerializeField]
        private MixController _mixController;
        [SerializeField]
        private SyringeAnimationController _syringeAnimationController;

        [SerializeField]
        private Rigidbody _cherpak;
        [SerializeField]
        private Transform _startPositionCherpak;
        [SerializeField]
        private Transform _endPositionCherpak;
        [SerializeField]
        private float _timeMove;

        [SerializeField]
        private ExperimentAnimation _experimentAnimation;
        [SerializeField]
        private Transform _monsterSpawn;
        [SerializeField]
        private GameObject _enemy;

        [Inject]
        private CraftController _craftController;
        [Inject]
        private ModelColorProvider _modelColorProvider;
        [Inject]
        private MonsterSpawner _monsterSpawner;

        private Location _currentLocation;
        private CompositeDisposable _subscription;
        private CameraMovementController _cameraMovementController;
        private Camera _camera;
        private Sequence _sequence;
        private Boiler _boiler;

        private void OnEnable()
        {
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventFinish>(StartMiningState),
                EventStreams.UserInterface.Subscribe<EventGenerateShooter>(Continue),
                EventStreams.UserInterface.Subscribe<EventCreateLocation>(SetFPSLocation)
            };
            
            _cherpak.gameObject.SetActive(false);
            _enemy.gameObject.SetActive(true);
        }

        private void OnDisable()    
        {
            _subscription?.Dispose();
        }

        private void StartMiningState(EventFinish eventFinish)
        {
            _boiler = eventFinish.Boiler;
            var movementController = _boiler.PlayerController;

            _camera = _boiler.Camera;
            _cameraMovementController = _camera.GetComponent<CameraMovementController>();
            
            _boiler.BoilDownCollider.SetTrigger();
            foreach (var boilerReagent in _boiler.Reagents)
            {
                boilerReagent.LockYPosition();
            }
            
            movementController.MoveToPoint(_boilerPosition, _timeMoveBoiler, () =>
            {
                _cameraMovementController.MoveToPoint(_mixingChamberCameraPosition, _timeMoveCamera, () =>
                {
                    _sequence = DOTween.Sequence();
                    _cherpak.transform.position = _startPositionCherpak.position;
                    _cherpak.isKinematic = true;
                    _cherpak.gameObject.SetActive(true);

                    _sequence.Append(_cherpak.transform.DOMove(_endPositionCherpak.position, _timeMove));
                    _sequence.AppendCallback(() =>
                    {
                        _cherpak.isKinematic = false;
                        _mixController.StartMixing(_boiler);
                        _mixController.OnEndMixing += FillSyringe;
                    });
                });
            });
        }

        private void FillSyringe(Color color)
        {
            _mixController.OnEndMixing -= FillSyringe;
            
            _sequence = DOTween.Sequence();
            _sequence.Append(_cherpak.transform.DOMove(_startPositionCherpak.position, _timeMove));
            _syringeAnimationController.StartFill(color);
            _syringeAnimationController.OnFillSyringe += MoveCameraToExperimental;
        }
        
        private void MoveCameraToExperimental()
        {
            _syringeAnimationController.OnFillSyringe -= MoveCameraToExperimental;
            _cameraMovementController.SplineFollower.spline = _splineComputerCamera;
            _cameraMovementController.SplineFollower.enabled = true;
                            
            ChangeSpeedCameraMove(_speedCameraFollower);
        }

        [UsedImplicitly]
        public void ShowExperimental()
        {
            _experimentAnimation.OnFill += CreateMonster;
            var color = _modelColorProvider.GetColorByReagentTypeToExperimental(_craftController.CurrentModelType);
            _experimentAnimation.StartExperiment(color, ShowScreenChoice);
        }

        private void CreateMonster()
        {
            _experimentAnimation.OnFill -= CreateMonster;
            var monster = _monsterSpawner.GetMonster();
            monster.transform.parent = _monsterSpawn;
            monster.transform.localPosition = Vector3.zero;
            monster.transform.localRotation = Quaternion.Euler(Vector3.zero);
            monster.NavMeshAgent.enabled = false;
            _enemy.gameObject.SetActive(false);
        }

        private void Continue(EventGenerateShooter eventStartShooter)
        {
            ChangeSpeedCameraMove(_speedCameraFollower);
        }

        private void SetFPSLocation(EventCreateLocation eventCreateLocation)
        {
            _currentLocation = eventCreateLocation.Location;
        }
        
        private void ShowScreenChoice()
        {
            EventStreams.UserInterface.Publish(new EventShowScreenChoice());
        }

        [UsedImplicitly]
        public void ChangeSpeedCameraMove(float speed)
        {
            _cameraMovementController.SplineFollower.followSpeed = speed;
        }

        [UsedImplicitly]
        public void SetRotation(float rotation)
        {
           _cameraMovementController.SetFollowerRotation(new Vector3(0, rotation, 0));
        }

        [UsedImplicitly]
        public void HideRunner()
        {
            _currentLocation.Camera.gameObject.SetActive(true);
            EventStreams.UserInterface.Publish(new EventReleaseRunner());
            EventStreams.UserInterface.Publish(new EventStartFPS());
        }
    }
}