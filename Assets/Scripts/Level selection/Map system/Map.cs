using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Level_selection.Bestiary_system;
using SaveAndLoadSystem;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;

namespace Level_selection.Map_system
{
    public class Map : IDisposable
    {
        public Region CurrentRegion => _regions.FirstOrDefault(region => region.Information.WorldType == _currentWorldType);
        public List<Region> Regions => _regions;

        private readonly Save _save;
        private readonly List<Region> _regions;
        private readonly Bestiary _bestiary;
        private readonly CompositeDisposable _subscription;
        private readonly MapPlayer _mapPlayer;
        private readonly int _countToOpenRegion;
        private readonly GameObject _map;
        private readonly ColorRegionProvider _colorRegionProvider;
        private float _timeChangeColor;

        private WorldType _currentWorldType;

        public Map(Settings settings, Bestiary bestiary, Save save, Load load, ColorRegionProvider colorRegionProvider)
        {
            _timeChangeColor = settings.TimeChangeColorRegion;
            _mapPlayer = settings.MapPlayer;
            _regions = settings.RegionsSequence;
            _bestiary = bestiary;
            _bestiary.Map = this;
            _save = save;
            _colorRegionProvider = colorRegionProvider;
            _countToOpenRegion = settings._countToOpenRegion;
            _map = settings.Map;

            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventCompletedLevel>(SaveLevels),
                EventStreams.UserInterface.Subscribe<EventOpenMap>(OpenEvent),
                EventStreams.UserInterface.Subscribe<EventPlay>(Hide),
                EventStreams.UserInterface.Subscribe<EventTeleportPlayerToRegion>(TeleportPlayerToRegion),
                EventStreams.UserInterface.Subscribe<EventOpenNewModel>(SaveLevels)
            };

            _currentWorldType = load.HasCurrentRegion()
                ? load.GetCurrentWorld()
                : settings.StartWorldType;

            var loadRegion = load.GetLevel();
            
            foreach (var region in _regions)
            {
                region.SetDefaultColors();
            }
            
            if (loadRegion != null)
            {
                FillSaveRegion(loadRegion);
            }
            else
            {
                foreach (var region in _regions)
                {
                    if (region.Information.RegionType == Region.RegionType.Neutral)
                    {
                        region.SetBlockColor(_colorRegionProvider.GetColorByRegionType(region.Information.RegionType));
                    }
                }
            }
            
            TeleportPlayerToCurrentRegion();
        }

        public Region GetRegionByWorldType(WorldType worldType)
        {
            return _regions.FirstOrDefault(region => region.Information.WorldType == worldType);
        }

        private void OpenEvent(EventOpenMap eventOpenMap)
        {
            Open();
            if (_bestiary.GetCountOpenModelByWorldType(_currentWorldType) >= _countToOpenRegion)
            {
                var currentRegion = CurrentRegion;

                if (!currentRegion.Information.IsPlayAnimation)
                {
                    currentRegion.Information.Region.ChangeColor(_timeChangeColor);
                }

                var nextRegion = GetNextRegion();
                if (nextRegion != null && !nextRegion.Information.IsPlayAnimation)
                {
                    if (!eventOpenMap.IsOpenGame)
                    {
                        nextRegion.Information.Region.ChangeColor(_timeChangeColor);
                    }
                    
                    EventStreams.UserInterface.Publish(new EventHideMapButtons());
                    _currentWorldType = nextRegion.Information.WorldType;
                    nextRegion.Information.IsPlayAnimation = true;
                    _mapPlayer.MoveToPoint(nextRegion.Information.Region.Point, () =>
                    {
                        EventStreams.UserInterface.Publish(new EventOpenMapButtons());
                    });
                    
                    _save.SaveCurrentRegion(CurrentRegion);
                    _save.SaveRegion(_regions);
                }
            }
            
            SetTeleportFXRegion();
        }

        private void Open()
        {
            _map.SetActive(true);
        }

        private void Hide(EventPlay eventPlay)
        {
            _map.SetActive(false);
        }

        private void SaveLevels(EventCompletedLevel eventCompletedLevel)
        {
            CurrentRegion.SetTheUnlockLevel();
            SaveLevels();
        }

        private void SaveLevels(EventOpenNewModel eventOpenNewModel)
        {
            SaveLevels();
        }

        private void SaveLevels()
        {
            if (_bestiary.GetCountOpenModelByWorldType(_currentWorldType) == _countToOpenRegion)
            {
                CurrentRegion.Information.RegionType = Region.RegionType.Passed;

                var nextRegion = GetNextRegion();
                if (nextRegion != null && nextRegion.Information.RegionType == Region.RegionType.Neutral)
                {
                    nextRegion.Information.RegionType = Region.RegionType.InProcess;
                    EventStreams.UserInterface.Publish(new EventOpenNewRegion());
                }
            }

            _save.SaveRegion(_regions);
        }

        private Region GetNextRegion()
        {
            for (var i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].Information.WorldType == _currentWorldType)
                {
                    if (i + 1 >= _regions.Count)
                    {
                        break;
                    }
                        
                    return _regions[i + 1];
                }
            }

            return null;
        }

        private void TeleportPlayerToCurrentRegion()
        {
            _mapPlayer.TeleportToPoint(CurrentRegion.Point);
            SetTeleportFXRegion();
        }

        private void SetTeleportFXRegion()
        {
            var currentRegion = CurrentRegion;
            currentRegion.SetActivateTeleportParticle(false);
            foreach (var region in _regions)
            {
                if (currentRegion.Information.WorldType == region.Information.WorldType)
                {
                    continue;
                }


                region.SetActivateTeleportParticle(region.Information.RegionType != Region.RegionType.Neutral);
            }
        }

        private void TeleportPlayerToRegion(EventTeleportPlayerToRegion eventTeleportPlayerToRegion)
        {
            var worldType = eventTeleportPlayerToRegion.Region.Information.WorldType;

            if (worldType == _currentWorldType || eventTeleportPlayerToRegion.Region.Information.RegionType == Region.RegionType.Neutral)
            {
                return;
            }
            
            _mapPlayer.TeleportToPoint(GetRegionByWorldType(worldType).Point);
            _currentWorldType = worldType;
            _save.SaveCurrentRegion(CurrentRegion);
            SetTeleportFXRegion();
            EventStreams.UserInterface.Publish(new EventPlayerZoom());
        }

        private void FillSaveRegion(List<Region.RegionInformation> regions)
        {
            foreach (var saveRegion in regions)
            {
                foreach (var region in _regions)
                {
                    if (region.Information.WorldType == saveRegion.WorldType)
                    {
                        region.Information.CurrentLevel = saveRegion.CurrentLevel;
                        region.Information.UnlockLevel = saveRegion.UnlockLevel;
                        region.Information.RegionType = saveRegion.RegionType;
                        region.Information.IsPlayAnimation = saveRegion.IsPlayAnimation;
                        region.Information.AllLevelCompleted = saveRegion.AllLevelCompleted;

                        if (region.Information.RegionType == Region.RegionType.Neutral)
                        {
                            region.Information.Region.SetBlockColor(_colorRegionProvider.GetColorByRegionType(region.Information.RegionType));
                        }
                        else
                        {
                            region.Information.Region.ActivateOnAwakeParticle();
                        }
                    }
                }
            }
        }

        [Serializable]
        public class Settings
        {
            public WorldType StartWorldType;
            public int _countToOpenRegion;
            public List<Region> RegionsSequence = new List<Region>();
            public MapPlayer MapPlayer;
            public GameObject Map;
            public float TimeChangeColorRegion = 1f;
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}