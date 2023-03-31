using System;
using DG.Tweening;
using Dreamteck.Splines;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;

namespace CameraComponent
{
	public class CameraMovementController: MonoBehaviour
	{
		public SplineFollower SplineFollower => _splineFollower;
		
		[SerializeField]
		private Rigidbody _target;
		[SerializeField]
		private float _followSpeed;
		[SerializeField]
		private Vector3 _offset;
		[SerializeField]
		private SplineFollower _splineFollower;
		[SerializeField]
		private Vector3 _rotation;

		[SerializeField]
		private bool _trackingEnable = true;
		private IDisposable _subscription;
		private Sequence _sequence;

		private void Awake()
		{
			_splineFollower.enabled = false;
		}

		private void OnEnable()
		{
			_splineFollower.motion.applyRotationX = true;
			_splineFollower.motion.applyRotationY = true;
			_splineFollower.motion.applyRotationZ = true;
			
			_splineFollower.enabled = false;
			_splineFollower.spline = null;
			_trackingEnable = true;
			_splineFollower.startPosition = 0f;
			_splineFollower.SetDistance(0);
			
			transform.position = _target.position + _offset;
			transform.rotation = Quaternion.Euler(_rotation);
		}

		public void SetFollowerRotation(Vector3 rotation)
		{
			_splineFollower.motion.applyRotationX = false;
			_splineFollower.motion.applyRotationY = false;
			_splineFollower.motion.applyRotationZ = false;

			_sequence = DOTween.Sequence();
			_sequence.Append(transform.DORotate(rotation, 1f));
		}

		private void OnDestroy()
		{
			_subscription?.Dispose();
		}
		
		public void MoveToPoint(Transform point, float time, Action callBack)
		{
			_trackingEnable = false;

			_sequence = DOTween.Sequence();
			_sequence.Append(transform.DOMove(point.position, time)).SetUpdate(UpdateType.Normal).SetEase(Ease.Linear);
			_sequence.Join(transform.DORotateQuaternion(point.rotation, time).SetUpdate(UpdateType.Normal).SetEase(Ease.Linear));
			_sequence.AppendCallback(() => callBack?.Invoke());
		}
		
		private void LateUpdate()
		{
			if (_trackingEnable)
			{
				transform.position = Vector3.Slerp(transform.position, _target.position + _offset,
					_followSpeed * Time.deltaTime);

				transform.rotation = Quaternion.Euler(_rotation);
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (_trackingEnable)
			{
				transform.position = _target.position + _offset;
				
				transform.rotation = Quaternion.Euler(_rotation);
			}
		}
	}
}