using System.Collections.Generic;
using UnityEngine;

namespace FPS.GenerationSystem
{
    public class Location : MonoBehaviour
    {
        public Camera Camera => _camera;
        public List<StripPoint> StripPoint => _stripPoint;
        public NavigationBaker NavigationBaker => _navigationBaker;

        [SerializeField]
        private FPSPlayer _fpsPlayer;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private List<StripPoint> _stripPoint = new List<StripPoint>();
        [SerializeField]
        private NavigationBaker _navigationBaker;

        private void Awake()
        {
            foreach (var stripPoint in _stripPoint)
            {
                stripPoint.Initialize(_fpsPlayer);
            }
        }
    }
}