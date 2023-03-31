using System;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level_selection.Map_system
{
    public class MapCamera : MonoBehaviour
    {
        public event Action OnClick;

        public Camera Camera => _camera;

        [SerializeField]
        private float _timeToClick;
        [SerializeField]
        private MapPlayer _mapPlayer;
        [SerializeField]
        private Vector3 _offset;
        [SerializeField]
        private Vector3 _rotation;
        [SerializeField]
        private float _followSpeed;
        [SerializeField]
        private Camera _camera;
        [SerializeField] 
        private float _speedDrag = 1f;
        [SerializeField]
        private float _upperBound;
        [SerializeField]
        private float _bottomBound;
        [SerializeField]
        private float _leftBound;
        [SerializeField]
        private float _rightBound;
        [SerializeField]
        private float _upBound;
        
        private float _startTimeClick;

        private void OnEnable()
        {
            transform.position = _mapPlayer.transform.position + _offset;
            transform.rotation = Quaternion.Euler(_rotation);
        }

        private void Update()
        {
            if (_mapPlayer.IsMove)
            {
                transform.position = Vector3.Slerp(transform.position, _mapPlayer.transform.position + _offset,
                    _followSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Euler(_rotation);
                return;
            }
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _startTimeClick = Time.time;
            }

            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && Time.time - _startTimeClick < _timeToClick)
            {
                OnClick?.Invoke();
            }

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var newPosition = new Vector3();
                newPosition.x = Input.GetAxis("Mouse X") * _speedDrag * Time.fixedDeltaTime;
                newPosition.z = Input.GetAxis("Mouse Y") * _speedDrag * Time.fixedDeltaTime;

                transform.Translate(newPosition, Space.World);
                ClampTransform();
            }
#else

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    _startTimeClick = Time.time;
                }

                if (touch.phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject(touch.fingerId)
                                                    && Time.time - _startTimeClick < _timeToClick)
                {
                    OnClick?.Invoke();
                }

                if (touch.phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    var newPosition = new Vector3();
                    newPosition.x = Input.GetAxis("Mouse X") * _speedDrag * Time.fixedDeltaTime;
                    newPosition.z = Input.GetAxis("Mouse Y") * _speedDrag * Time.fixedDeltaTime;

                    transform.Translate(newPosition, Space.World);
                    ClampTransform();
                }
            }
#endif
        }

        private void ClampTransform()
        {
            var clampX = Mathf.Clamp(transform.position.x, _leftBound, _rightBound);
            var clampY = Mathf.Clamp(transform.position.z, _bottomBound, _upperBound);
            transform.position = new Vector3(clampX, transform.position.y, clampY);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(_leftBound, _upBound, _upperBound),
                new Vector3(_rightBound, _upBound, _upperBound));
            Gizmos.DrawLine(new Vector3(_leftBound, _upBound, _bottomBound),
                new Vector3(_rightBound, _upBound, _bottomBound));
            Gizmos.DrawLine(new Vector3(_leftBound, _upBound, _upperBound),
                new Vector3(_leftBound, _upBound, _bottomBound));
            Gizmos.DrawLine(new Vector3(_rightBound, _upBound, _upperBound),
                new Vector3(_rightBound, _upBound, _bottomBound));
            transform.position = Vector3.Slerp(transform.position, _mapPlayer.transform.position + _offset,
                _followSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(_rotation);
        }
#endif
    }
}