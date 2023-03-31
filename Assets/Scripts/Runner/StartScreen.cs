using System;
using DG.Tweening;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runner
{
    public class StartScreen : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
    {
        [SerializeField]
        private Vector3 _twoStateScale;
        [SerializeField]
        private float _timeChangeScale;
        [SerializeField]
        private RectTransform _text;
        [SerializeField]
        private GameObject _hideButton;
        
        private Sequence _sequence;
        private IDisposable _subscription;
        private Vector3 _startScale;
        
        private void Awake()
        {
            _subscription = EventStreams.UserInterface.Subscribe<EventGenerationRunner>(Show);
            _startScale = _text.localScale;
        }

        private void OnEnable()
        {
            _text.localScale = _startScale;
            _sequence = DOTween.Sequence();
            _sequence.Append(_text.transform.DOScale(_twoStateScale, _timeChangeScale)).SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        private void Show(EventGenerationRunner eventGenerationRunner)
        {
            gameObject.SetActive(true);
            _hideButton.SetActive(true);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.SetActive(false);
            _hideButton.SetActive(false);
            EventStreams.UserInterface.Publish(new EventStartRunner());
        }
    }
}