using System.Collections.Generic;
using LiquidVolumeFX;
using UnityEngine;

namespace Runner.PlayerComponent
{
    public class BoilerAnimationController : MonoBehaviour
    {
        [SerializeField]
        private LiquidVolume _liquidVolume;

        [SerializeField]
        private ParticleSystem _removeReagent;
        [SerializeField]
        private ParticleSystem _addReagent;
        [SerializeField]
        private List<ParticleSystem> _changeColorAddFX = new List<ParticleSystem>();
        [SerializeField]
        private List<ParticleSystem> _changeColorRemoveFX = new List<ParticleSystem>();
        
        public void ShowAddReagent()
        {
            foreach (var system in _changeColorAddFX)
            {
                var systemMain = system.main;
                systemMain.startColor = _liquidVolume.LiquidColor1;
            }

            _addReagent.Play();
        }

        public void ShowRemoveReagent()
        {
            foreach (var system in _changeColorRemoveFX)
            {
                var systemMain = system.main;
                systemMain.startColor = _liquidVolume.LiquidColor1;
            }
            
            _removeReagent.Play();
        }
    }
}