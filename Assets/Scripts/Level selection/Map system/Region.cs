using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Runner.GeneratorSystem;
using UnityEngine;

namespace Level_selection.Map_system
{
    public class Region : MonoBehaviour
    {
        public Transform Point => _point;
        public RegionInformation Information => _regionInformation;
        public Transform UiAttachPoint => _uiAttachPoint;
        
        [SerializeField]
        private Transform _point;
        [SerializeField]
        private Transform _uiAttachPoint;
        [SerializeField]
        private RegionInformation _regionInformation;
        [SerializeField]
        private List<PartStrip> _partStrips = new List<PartStrip>();
        [SerializeField]
        private MeshRenderer _region;
        [SerializeField]
        private ParticleSystem _teleportParticle;
        [SerializeField]
        private List<ParticleSystem> _activateParticle = new List<ParticleSystem>();
        [SerializeField]
        private List<MeshRenderer> _changeColor = new List<MeshRenderer>();

        private readonly List<Color> _defaultColors = new List<Color>();

        public void SetDefaultColors()
        {
            foreach (var renderer in _changeColor)
            {
                foreach (var rendererMaterial in renderer.materials)
                {
                    _defaultColors.Add(rendererMaterial.color);
                }
            }
        }

        public void SetActivateTeleportParticle(bool isActivate)
        {
            _teleportParticle.gameObject.SetActive(isActivate);
        }
        
        public PartStrip GetCurrentPartStrip()
        {
            if (_regionInformation.CurrentLevel >= _partStrips.Count)
            {
                _regionInformation.CurrentLevel = 0;
            }

            var partStrip = _partStrips[_regionInformation.CurrentLevel];
            return partStrip;
        }

        public PartStrip GetUnlockLevel()
        {
            if (_regionInformation.UnlockLevel >= _partStrips.Count)
            {
                _regionInformation.AllLevelCompleted = true;
                _regionInformation.UnlockLevel = 0;
            }

            _regionInformation.CurrentLevel = _regionInformation.UnlockLevel;
            var partStrip = _partStrips[_regionInformation.CurrentLevel];
            return partStrip;
        }

        public void SetTheUnlockLevel()
        {
            _regionInformation.UnlockLevel = _regionInformation.CurrentLevel + 1;
        }

        public void SetBlockColor(Color color)
        {
            foreach (var meshRenderer in _changeColor)
            {
                foreach (var meshRendererMaterial in meshRenderer.materials)
                {
                    meshRendererMaterial.color = color;
                }
            }
        }

        public void ActivateOnAwakeParticle()
        {
            foreach (var system in _activateParticle)
            {
                var main = system.main;
                main.playOnAwake = true;
                main.prewarm = true;
            }
        }

        public async UniTaskVoid ChangeColor(float timeChange)
        {
            var currentTime = 0f;
            var startColor = _region.material.color;
            
            foreach (var system in _activateParticle)
            {
                system.Play();
            }
            
            while (currentTime <= timeChange)
            {
                currentTime += Time.deltaTime;
                var progress = currentTime / timeChange;
                
                var index = 0;
                foreach (var renderer in _changeColor)
                {
                    foreach (var material in renderer.materials)
                    {
                        var color = Color.Lerp(startColor, _defaultColors[index], progress);
                        material.color = color;
                        index++;
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            }
        }

        public enum RegionType
        {
            Neutral = 0,
            Passed = 1,
            InProcess = 2
        }

        [Serializable]
        public class RegionInformation
        {
            [JsonIgnore]
            public Region Region;
            public WorldType WorldType;
            public RegionType RegionType;
            public bool IsPlayAnimation;

            [HideInInspector]
            public bool AllLevelCompleted;
            [HideInInspector]
            public int CurrentLevel;
            [HideInInspector]
            public int UnlockLevel;
        }
    }
}