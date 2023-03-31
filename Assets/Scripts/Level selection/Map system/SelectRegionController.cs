using System;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;

namespace Level_selection.Map_system
{
    public class SelectRegionController : MonoBehaviour
    {
        [SerializeField]
        private MapCamera _mapCamera;
        [SerializeField]
        private LayerMask _layerMaskRaycast;

        private void Awake()
        {
            _mapCamera.OnClick += TeleportToRegion;
        }

        private void TeleportToRegion()
        {
            var ray = _mapCamera.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 10000f, _layerMaskRaycast))
            {
                if (hitInfo.collider.TryGetComponent(out Region region))
                {
                    EventStreams.UserInterface.Publish(new EventTeleportPlayerToRegion(region));
                }
            }
        }

        private void OnDestroy()
        {
            _mapCamera.OnClick -= TeleportToRegion;
        }
    }
}