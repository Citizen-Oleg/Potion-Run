using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using Joystick_and_Swipe;
using Runner.PlayerComponent;
using Tools.SimpleEventBus;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private Vector3 _rotationLeft;
    [SerializeField]
    private Vector3 _rotationRight;
    [SerializeField]
    float m_CustomPlayerSpeed = 10.0f;
    [SerializeField]
    float m_AccelerationSpeed = 10.0f;
    [SerializeField]
    float m_DecelerationSpeed = 20.0f;
    [SerializeField]
    float m_HorizontalSpeedFactor = 0.5f;
    [SerializeField]
    private float _levelWidth;

    private Transform m_Transform;
    private bool m_HasInput;
    private float m_MaxXPosition;
    private float m_XPos;
    private float m_ZPos;
    private float m_TargetPosition;
    private float m_Speed;
    private float m_TargetSpeed;
    private float _currentX;
    private Quaternion _startRotation;

    private const float k_HalfWidth = 0.5f;
    
    private Sequence _sequence;
    private DynamicJoystick _variableJoystick;
    
    [SerializeField]
    private bool _isActiveRunnerMovement;

    private IDisposable _subscription;
    
    void Awake()
    {
        _subscription = EventStreams.UserInterface.Subscribe<EventStartRunner>(start => _isActiveRunnerMovement = true);
    }

    private void OnEnable()
    {
        SetPositions();
    }

    public void Initialize(DynamicJoystick variableJoystick)
    {
        _variableJoystick = variableJoystick;
    }

    private void SetPositions()
    {
        m_Transform = transform;

        m_MaxXPosition = _levelWidth * k_HalfWidth;
        m_ZPos = transform.position.z;
        m_XPos = transform.position.x;
        
        ResetSpeed();
    }
    
    public float GetDefaultSpeed()
    {
        return m_CustomPlayerSpeed;
    }

    public void ResetSpeed()
    {
        m_Speed = 0.0f;
        m_TargetSpeed = GetDefaultSpeed();
    }


    public void SetDeltaPosition()
    {
        float fullWidth = m_MaxXPosition * 2.0f;
        m_TargetPosition += fullWidth * _variableJoystick.Horizontal;
        m_TargetPosition = Mathf.Clamp(m_TargetPosition, -m_MaxXPosition, m_MaxXPosition);
        m_HasInput = true;
    }

    public void MoveToPoint(Transform point, float time, Action callback)
    {
        _isActiveRunnerMovement = false;
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(point.position, time).SetEase(Ease.Linear).SetUpdate(UpdateType.Normal));
        _sequence.AppendCallback(() => callback?.Invoke());
    }

    private void Update()
    {
        if (!_isActiveRunnerMovement)
        {
            var rotationDefault = Quaternion.identity;
            m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, rotationDefault, _rotationSpeed * Time.deltaTime);
            return;
        }

        SetDeltaPosition();
        float deltaTime = Time.deltaTime;

        // Update Speed

        if (!m_HasInput)
        {
            Decelerate(deltaTime, 0.0f);
        }
        else if (m_TargetSpeed < m_Speed)
        {
            Decelerate(deltaTime, m_TargetSpeed);
        }
        else if (m_TargetSpeed > m_Speed)
        {
            Accelerate(deltaTime, m_TargetSpeed);
        }

        float speed = m_Speed * deltaTime;

        // Update position

        m_ZPos += speed;
        
        var t = 0f;
        var rotation = Quaternion.identity;
        
        if (m_HasInput && (_variableJoystick.Horizontal > 0.01f || _variableJoystick.Horizontal < -0.01f))
        {
            float horizontalSpeed = speed *  Mathf.Lerp(0, m_HorizontalSpeedFactor, Mathf.Abs(_variableJoystick.Horizontal));

            float newPositionTarget = Mathf.Lerp(m_XPos, m_TargetPosition, horizontalSpeed);
            float newPositionDifference = newPositionTarget - m_XPos;

            newPositionDifference = Mathf.Clamp(newPositionDifference, -horizontalSpeed, horizontalSpeed);

            m_XPos += newPositionDifference;
            
            t = Mathf.Abs(_variableJoystick.Horizontal);
            t *= _rotationSpeed * Time.deltaTime;
            rotation = Quaternion.Euler(_variableJoystick.Horizontal > 0 ? _rotationRight : _rotationLeft);
        }
        else
        {
            rotation = Quaternion.identity;
            t = _rotationSpeed * Time.deltaTime;
        }

        m_Transform.position = new Vector3(m_XPos, m_Transform.position.y, m_ZPos);
        m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, rotation, t);
    }
    
    void Accelerate(float deltaTime, float targetSpeed)
    {
        m_Speed += deltaTime * m_AccelerationSpeed;
        m_Speed = Mathf.Min(m_Speed, targetSpeed);
    }

    void Decelerate(float deltaTime, float targetSpeed)
    {
        m_Speed -= deltaTime * m_DecelerationSpeed;
        m_Speed = Mathf.Max(m_Speed, targetSpeed);
    }

    private void OnDestroy()
    {
        _subscription?.Dispose();
    }
}
