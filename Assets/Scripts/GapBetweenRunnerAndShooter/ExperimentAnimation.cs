using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Events;
using GapBetweenRunnerAndShooter.CraftSystem;
using JetBrains.Annotations;
using LiquidVolumeFX;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace GapBetweenRunnerAndShooter
{
    public class ExperimentAnimation : MonoBehaviour
    {
        public event Action OnFill;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Transform _lever;
        [SerializeField]
        private SyringeAnimationController _syringeAnimationController;
        [FormerlySerializedAs("_liquidVolume")]
        [SerializeField]
        private LiquidVolume _liquidVolumeWater;
        [SerializeField]
        private Transform _water;
        [SerializeField]
        private Transform _startPosition;
        [SerializeField]
        private Transform _endPosition;
        [SerializeField]
        private float _timeChangeColor;
        [SerializeField]
        private float _timeChangeAlpha;
        [SerializeField]
        private float _timeFill;

        [SerializeField]
        private List<ParticleSystem> _confetti = new List<ParticleSystem>();

        [Inject]
        private CraftController _craftController;
        [Inject]
        private ModelColorProvider _modelColorProvider;

        private Color _startColorWater;
        private Color _defaultColor;
        private float _defaultLevel;
        private Action _callBack;
        private Quaternion _defaultRotationLever;
        private MeshRenderer _meshRenderer;
        private IDisposable _subscription;

        private void Awake()
        {
            _meshRenderer = _water.GetComponent<MeshRenderer>();
            _startColorWater = _meshRenderer.material.color;
            _defaultColor = _liquidVolumeWater.LiquidColor1;
            _defaultLevel = _liquidVolumeWater.level; 
            _defaultRotationLever = _lever.localRotation;
            _subscription = EventStreams.UserInterface.Subscribe<EventOpenNewModel>(ShowConfetti);
        }

        private void OnEnable()
        {
            _animator.enabled = false;
            _liquidVolumeWater.LiquidColor1 = _defaultColor;
            _liquidVolumeWater.level = _defaultLevel;
            _water.transform.position = _startPosition.position;
            
            _meshRenderer.material.color = _startColorWater;
            
            _lever.localRotation = _defaultRotationLever;
        }

        public void StartExperiment(Color color, Action callBack)
        {
            var newColor = new Color(color.r, color.g, color.b, 0);
            _meshRenderer.material.color = newColor;
            _callBack = callBack;
            StartAnimationMoveSyringe();
        }

        [UsedImplicitly]
        public void FillCapsule()
        {
            ShowExperiment();
        }

        private void StartAnimationMoveSyringe()
        {
            _animator.enabled = true;
            _animator.Play("MoveSyringe");
        }

        [UsedImplicitly]
        public async UniTaskVoid ChangeColor()
        {
            _syringeAnimationController.StartEmpty(_timeChangeColor);
            var currentTime = 0f;
            var targetColor =
                _modelColorProvider.GetColorByReagentTypeToExperimental(_craftController.CurrentModelType);

            while (currentTime < _timeChangeColor)
            {
                currentTime += Time.deltaTime;

                _liquidVolumeWater.LiquidColor1 = Color.Lerp(_defaultColor, targetColor, currentTime / _timeChangeColor);
                
                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
            
            _animator.Play("StartExperiment");
        }

        private async UniTaskVoid ShowExperiment()
        {
            var currentTime = 0f;

            while (currentTime < _timeChangeAlpha)
            {
                currentTime += Time.deltaTime;

                var color = _meshRenderer.material.color;
                var a = Mathf.Lerp(0f, 1f, currentTime / _timeChangeAlpha);
                color.a = a;

                _meshRenderer.material.color = color;
                
                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }

            currentTime = 0f;
            
            while (currentTime < _timeFill)
            {
                currentTime += Time.deltaTime / _timeFill;

                _water.position = Vector3.Lerp(_startPosition.position, _endPosition.position, currentTime  / _timeFill);
                _liquidVolumeWater.level = Mathf.Lerp(_defaultLevel, 0f, currentTime  / _timeFill);
                
                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
            
            OnFill?.Invoke();
            
            currentTime = 0f;
            
            while (currentTime < _timeFill)
            {
                currentTime += Time.deltaTime;

                _water.position = Vector3.Lerp(_endPosition.position, _startPosition.position, currentTime  / _timeFill);
                
                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
            
            EventStreams.UserInterface.Publish(new EventCreateMonster());

            currentTime = 0f;
            while (currentTime < _timeChangeAlpha)
            {
                currentTime += Time.deltaTime;

                var color = _meshRenderer.material.color;
                var a = Mathf.Lerp(1f, 0f, currentTime / _timeChangeAlpha);
                color.a = a;

                _meshRenderer.material.color = color;
                
                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
            
            _callBack?.Invoke();
        }

        private void ShowConfetti(EventOpenNewModel eventOpenNewModel)
        {
            foreach (var system in _confetti)
            {
                system.Play();
            }
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}