using System;
using System.Collections.Generic;
using Base.MoveAnimation._3D;
using Cysharp.Threading.Tasks;
using LiquidVolumeFX;
using Runner.PlayerComponent;
using Runner.ReagentSystem;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Runner
{
    public class Boiler : MonoBehaviour
    {
        public Camera Camera => _camera;
        public List<Reagent> Reagents => _reagents;
        public PlayerController PlayerController => _playerController;
        public LiquidVolume LiquidVolume => _liquidVolume;
        public ParticleSystemRenderer ParticleSystemRenderer => _particleSystemRenderer;
        public ParticleSystem Bubble => _bubble;
        public ParticleSystem Star => _star;
        public ParticleSystem Fog => _fog;
        public BoilDownCollider BoilDownCollider => _boilDownCollider;
        
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private LiquidVolume _liquidVolume;
        [SerializeField]
        private float _timeChangeColor;
        [SerializeField]
        private Transform _spawnPosition;
        [SerializeField]
        private BoilerAnimationController _boilerAnimationController;
        [Header("Remove settings")]
        [SerializeField]
        private float _flightTime;
        [SerializeField]
        private Transform _throwoutPoint;
        [SerializeField]
        private Transform _throwoutPointTwo;
        [Header("Particle")]
        [SerializeField]
        private ParticleSystem _bubble;
        [SerializeField]
        private ParticleSystem _fog;
        [SerializeField]
        private ParticleSystem _star;
        [SerializeField]
        private BoilDownCollider _boilDownCollider;

        private int _throwoutNumber;
        private ParticleSystemRenderer _particleSystemRenderer;
        private float _startLevelWater;
        private Color _startColorWater;
        private Color _startColorBubble;
        private Color _startColorFog;
        private float _startBubbleEmission;
        private float _startStartEmission;

        private readonly List<Reagent> _reagents = new List<Reagent>();
        
        private ReagentModelProvider _reagentModelProvider;
        private AnimationManager _animationManager;

        private void Awake()
        {
            _particleSystemRenderer = _bubble.GetComponent<ParticleSystemRenderer>();
            _startColorBubble = _particleSystemRenderer.material.color;
            _startColorFog = _fog.main.startColor.color;
            _startBubbleEmission = _bubble.emission.rateOverTime.constant;
            _startStartEmission = _star.emission.rateOverTime.constant;
            _startColorWater = LiquidVolume.LiquidColor1;
            _startLevelWater = LiquidVolume.level;
        }

        private void OnEnable()
        {
            LiquidVolume.level = _startLevelWater;
            _fog.Play();
            _bubble.Play();
            _star.Play();
        }

        public void Initialize(ReagentModelProvider reagentModelProvider, AnimationManager animationManager, DynamicJoystick variableJoystick)
        {
            _reagentModelProvider = reagentModelProvider;
            _animationManager = animationManager;
            _playerController.Initialize(variableJoystick);
        }

        public void AddReagents(ReagentType reagentType)
        {
            _boilerAnimationController.ShowAddReagent();
            var reagent = _reagentModelProvider.GetReagentModel(reagentType);

            reagent.Rigidbody.isKinematic = false;
            reagent.transform.parent = _spawnPosition;
            reagent.transform.position = _spawnPosition.position;

            _reagents.Add(reagent);
        }

        public void RemoveLastReagentType()
        {
            if (_reagents.Count == 0)
            {
                return;
            }

            _boilerAnimationController.ShowRemoveReagent();

            var index = _reagents.Count - 1;
            var reagent = _reagents[index];
            _reagents.RemoveAt(index);
            
            var endPoint = _throwoutNumber % 2 == 0 ? _throwoutPoint : _throwoutPointTwo;
            _throwoutNumber++;
            reagent.ResetParent();
            _animationManager.ShowMoveObject(reagent.transform, endPoint, Vector3.zero, Vector3.zero,
                () =>
                {
                    reagent.Release();
                }, false, false, _flightTime);
        }

        public async UniTaskVoid ChangeColorWater(Color color)
        {
            var currentTime = 0f;
            var startColor = _liquidVolume.LiquidColor1;
            var startColorParticle = _particleSystemRenderer.material.color;

            while (currentTime < _timeChangeColor && _liquidVolume != null)
            {
                currentTime += Time.deltaTime;
                var progress = currentTime / _timeChangeColor;
                _liquidVolume.LiquidColor1 = Color.Lerp(startColor, color, progress);
                var colorParticle = Color.Lerp(startColorParticle, color, progress);
                colorParticle.a = startColorParticle.a;
                _particleSystemRenderer.sharedMaterial.color = colorParticle;
                
                var mainFog = _fog.main;
                colorParticle.a = mainFog.startColor.color.a;
                mainFog.startColor = colorParticle;

                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }

        public void ReleaseReagent()
        {
            foreach (var reagent in _reagents)
            {
                reagent.Release();
            }
            
            _reagents.Clear();
        }

        private void OnDisable()
        {
            _particleSystemRenderer.material.color = _startColorBubble;
            
            var mainFog = _fog.main;
            mainFog.startColor = _startColorFog;
            _fog.Stop();

            var bubbleEmission = _bubble.emission;
            bubbleEmission.rateOverTime = _startBubbleEmission;
            _bubble.Stop();

            var starEmission = _star.emission;
            starEmission.rateOverTime = _startStartEmission;
            _star.Stop();

            _liquidVolume.LiquidColor1 = _startColorWater;
            
            ReleaseReagent();
        }
    }
}