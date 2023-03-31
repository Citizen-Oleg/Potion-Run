using System;
using DG.Tweening;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GapBetweenRunnerAndShooter.ScreenChoice
{ 
    public class SyringeScreen : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IPointerUpHandler, IPointerEnterHandler
    {
        [SerializeField]
        private RectTransform _text;
        [SerializeField]
        private Vector3 _twoStateScale;
        [SerializeField]
        private float _timeChangeScale;
     
        private IDisposable _subscription;
        private Sequence _sequence;
        private Vector3 _startScale;
        
        private void Awake()
        {
            _subscription = EventStreams.UserInterface.Subscribe<EventStartSyringeFill>(Show);
            _startScale = _text.localScale;
            gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            _text.localScale = _startScale;
            _sequence = DOTween.Sequence();
            _sequence.Append(_text.transform.DOScale(_twoStateScale, _timeChangeScale)).SetLoops(-1, LoopType.Yoyo);
        }
        
        private void OnDisable()
        {
            _sequence?.Kill();
        }

        private void Show(EventStartSyringeFill eventStartSyringeFill)
        {
            gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}