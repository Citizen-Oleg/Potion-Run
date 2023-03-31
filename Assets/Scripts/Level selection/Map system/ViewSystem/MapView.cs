using System;
using Base.Tools;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Level_selection.Map_system.ViewSystem
{
    public class MapView : MonoBehaviour
    {
        public WorldType WorldType => _worldType;
        
        [SerializeField]
        private WorldType _worldType;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private float _timeShow;
        
        private RectTransform _currentTransform;
        private RectTransform _container;
        private Transform _attachPoint;
        private Camera _camera;
        private Sequence _sequence;

        public void Initialize(Transform uiAttachPosition, Camera camera)
        {
            _container = transform.parent as RectTransform;
            _currentTransform = transform as RectTransform;
            _attachPoint = uiAttachPosition;
            _camera = camera;
            LateUpdate();
        }
        
        public void SetPercent(float percent)
        {
            _text.text = (int) percent + "%";
        }

        public void Show()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(Vector3.one, _timeShow));
        }

        private void LateUpdate()
        {
            if (_attachPoint != null)
            {
                _currentTransform.anchoredPosition = UIUtility.WorldToCanvasAnchoredPosition(_camera, _container, _attachPoint.position);
            }
        }
    }
}