using System;
using System.Collections.Generic;
using System.Linq;
using Level_selection.Bestiary_system;
using UnityEngine;
using Zenject;

namespace Level_selection.Map_system.ViewSystem
{
    public class ViewManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Bestiary _bestiary;
        [SerializeField]
        private List<MapView> _mapViews = new List<MapView>();

        [Inject]
        private Map _map;

        private void Awake()
        {
            foreach (var mapView in _mapViews)
            {
                foreach (var mapRegion in _map.Regions.Where(mapRegion => mapView.WorldType == mapRegion.Information.WorldType))
                {
                    mapView.Initialize(mapRegion.UiAttachPoint, _camera);
                }
            }
        }

        private void OnEnable()
        {
            foreach (var mapRegion in _map.Regions)
            {
                foreach (var mapView in _mapViews)
                {
                    if (mapRegion.Information.RegionType == Region.RegionType.Neutral)
                    {
                        continue;
                    }

                    if (mapRegion.Information.WorldType == mapView.WorldType)
                    {
                        mapView.SetPercent(_bestiary.GetPercentOpenByWorldType(mapView.WorldType));
                        mapView.Show();
                    }
                }
            }
        }
    }
}