using System;
using System.Numerics;
using Events;
using GapBetweenRunnerAndShooter.CraftSystem;
using Runner;
using Runner.ReagentSystem;
using Tools.SimpleEventBus;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace GapBetweenRunnerAndShooter.MixingSystem
{
    public class MixController : MonoBehaviour
    {
        public event Action<Color> OnEndMixing;
        public float Progress => _currentDistance / _targetDistance;

        [SerializeField]
        private float _mixingSpeed;
        [SerializeField]
        private float _mixingTime;
        [SerializeField]
        private Rigidbody _stick;
        [SerializeField]
        private float _dopFloatProgress = 0.5f;
        [SerializeField]
        private float _emissionStarEnd = 15f;
        [SerializeField]
        private float _emissionBubbleEnd = 15f;
        [SerializeField]
        private Color _maxAlphaColorFog;

        [Inject]
        private CraftController _craftController;
        [Inject]
        private ModelColorProvider _modelColorProvider;

        private float _targetDistance;
        private float _currentDistance;
        private bool _isStart;

        private Boiler _boiler;
        private Color _startColorWater;
        private Color _endColorWater;
        
        private Color _startColorParticleBubble;
        private Color _endColorParticleBubble;

        private Color _fogStartColor;
        

        public void StartMixing(Boiler boiler)
        {
            EventStreams.UserInterface.Publish(new EventStartMixingReagent(this));
            _currentDistance = 0;
            _targetDistance = _mixingSpeed * _mixingTime;
           
            _boiler = boiler;
            _isStart = true;

            _startColorWater = _boiler.LiquidVolume.LiquidColor1;
            _fogStartColor = boiler.Fog.main.startColor.color;
            _startColorParticleBubble = _boiler.ParticleSystemRenderer.sharedMaterial.color;

            var endColor = _modelColorProvider.GetColorByReagentType(_craftController.CurrentModelType);
            _endColorWater = endColor;
            _endColorParticleBubble = endColor;
            _boiler.Star.Play();
        }

        private void Update()
        {
            MoveMix();
            
            if (!_isStart || _boiler == null || Progress < 0.05f)
            {
                return;
            }
            
            foreach (var boilerReagent in _boiler.Reagents)
            {
                boilerReagent.transform.localScale = Vector3.Lerp(boilerReagent.StartLocalScale, Vector3.zero, Progress);
            }

            _boiler.LiquidVolume.LiquidColor1 = Color.Lerp(_startColorWater, _endColorWater, Progress);
            var colorParticle = Color.Lerp(_startColorParticleBubble, _endColorParticleBubble, Progress);
            colorParticle.a = _startColorParticleBubble.a;
            _boiler.ParticleSystemRenderer.sharedMaterial.color = colorParticle;

            var fogColor = Color.Lerp(_fogStartColor, _endColorParticleBubble, Progress + _dopFloatProgress);
            var mainFog = _boiler.Fog.main;
            fogColor.a = Mathf.Clamp(fogColor.a, fogColor.a, _maxAlphaColorFog.a);
            mainFog.startColor = fogColor;
            
            var emissionStar = Mathf.Lerp(0, _emissionStarEnd, Progress + _dopFloatProgress);
            var emissionModule = _boiler.Star.emission;
            emissionModule.rateOverTime = emissionStar;
            
            var emissionBubble = Mathf.Lerp(0, _emissionBubbleEnd, Progress + _dopFloatProgress);
            var emissionModuleBubble = _boiler.Bubble.emission;
            emissionModuleBubble.rateOverTime = emissionBubble;
        }
    
        private void MoveMix()
        {
            if (!_isStart)
            {
                _stick.velocity = Vector3.zero;
                return;
            }
            
            if (Input.GetMouseButton(0))
            {
                var mouse = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")).normalized;
                var move = mouse * (_mixingSpeed * Time.fixedDeltaTime);
                _stick.velocity = move;
                _currentDistance += _stick.velocity.magnitude;
            }
            else
            {
                _stick.velocity = Vector3.zero;
            }

            if (Progress >= 1f)
            {
                _isStart = false;
                OnEndMixing?.Invoke(_endColorWater);
                _boiler.ReleaseReagent();
            }
        }
    }
}
