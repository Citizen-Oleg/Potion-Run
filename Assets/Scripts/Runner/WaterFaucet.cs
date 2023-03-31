using System;
using Runner.PlayerComponent;
using TMPro;
using UnityEngine;

namespace Runner
{
    [RequireComponent(typeof(Collider))]
    public class WaterFaucet : MonoBehaviour
    {
        [SerializeField]
        private float _addLevel;
        [SerializeField]
        private float _time;
        [SerializeField]
        private Color _color;
        
        private Boiler _boiler;

        private float _currentTime;

        private void Update()
        {
            if (_boiler != null && IsTime())
            {
                _boiler.LiquidVolume.level += _addLevel;
            }
        }

        private bool IsTime()
        {
            if (Time.time > _currentTime)
            {
                _currentTime = Time.time + _time;
                return true;
            }
            
            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BoilerCollider boiler))
            {
                _boiler = boiler.Boiler;
                _boiler.ChangeColorWater(_color);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out BoilerCollider boiler))
            {
                _boiler = null;
            }
        }
    }
}