using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Events;
using LiquidVolumeFX;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.Serialization;

namespace GapBetweenRunnerAndShooter
{
    public class SyringeAnimationController : MonoBehaviour
    {
        public float Progress => _currentDistance / _targetDistance;

        public event Action OnFillSyringe;

        [SerializeField]
        private float _travelTime;
        [SerializeField]
        private Transform _startPosition;
        [SerializeField]
        private Transform _endPosition;
        [SerializeField]
        private Transform _syringe;

        [SerializeField]
        private LiquidVolume _liquidVolume;
        [SerializeField]
        private Transform _pen;
        [SerializeField]
        private Transform _penFillPosition;
        [SerializeField]
        private Transform _penEmptyPosition;
        [FormerlySerializedAs("_mixingSpeed")]
        [SerializeField]
        private float _speedUp;
        [FormerlySerializedAs("_mixingTime")]
        [SerializeField]
        private float _timeUp;
        
        private float _targetDistance;
        private float _currentDistance;
        private bool _isStart;

        private Sequence _sequence;
        
        private void OnEnable()
        {
            _syringe.transform.position = _startPosition.position;
            _syringe.transform.rotation = _startPosition.rotation;
            _pen.position = _penEmptyPosition.position;
            _liquidVolume.level = 0;
            _syringe.gameObject.SetActive(false);
            
            _currentDistance = 0;
            _targetDistance = _speedUp * _timeUp;
        }

        public void StartFill(Color color)
        {
            _liquidVolume.LiquidColor1 = color;
            _syringe.gameObject.SetActive(true);

            _sequence = DOTween.Sequence();
            _sequence.Append(_syringe.DOMove(_endPosition.position, _travelTime).SetEase(Ease.Linear));
            _sequence.AppendCallback(() =>
            {
                EventStreams.UserInterface.Publish(new EventStartSyringeFill());
                _isStart = true;
            });
        }

        public async UniTaskVoid StartEmpty(float time)
        {
            var currentTime = 0f;

            while (currentTime < time)
            {
                currentTime += Time.deltaTime;
                var progress = currentTime / time;
                
                _pen.position = Vector3.Lerp(_penFillPosition.position, _penEmptyPosition.position, progress);
                _liquidVolume.level = Mathf.Lerp(1, 0, progress);

                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }

        private void Update()
        {
            if (!_isStart)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                var mouse = new Vector3(0, Input.GetAxis("Mouse Y"), 0);
           
                if (mouse.y < 0)
                {
                    return;
                }
                
                var move = mouse * (_speedUp * Time.fixedDeltaTime);
                _currentDistance += move.magnitude;

                _liquidVolume.level = Mathf.Lerp(0, 1f, Progress);
                _pen.position = Vector3.Lerp(_penEmptyPosition.position, _penFillPosition.position, Progress);
            }

            if (Progress >= 1f)
            {
                _isStart = false;
                OnFillSyringe?.Invoke();
            }
        }
    }
}