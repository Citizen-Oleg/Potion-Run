using System;
using System.Collections.Generic;
using Events;
using Level_selection.Map_system;
using Runner.GeneratorSystem;
using Runner.ReagentSystem;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class LevelManager : IDisposable
    {
        public ReagentType CurrentRewardReagentType => _map.CurrentRegion.GetCurrentPartStrip().RewardReagent;
        
        private readonly GameObject _canvasMap;
        private readonly GameObject _canvasGame;
        private readonly Map _map;
        private readonly CompositeDisposable _subscription;
        private readonly StripGenerator _stripGenerator;
        private readonly List<PlaneWorldType> _planeWorldTypes;

        public LevelManager(Settings settings, Map map, StripGenerator stripGenerator)
        {
            _planeWorldTypes = settings.PlaneWorldTypes;
            _canvasGame = settings.Game;
            _map = map;
            _stripGenerator = stripGenerator;
            
            _canvasGame.SetActive(false);
            
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventRestart>(Restart),
                EventStreams.UserInterface.Subscribe<EventOpenMap>(OpenMap),
                EventStreams.UserInterface.Subscribe<EventContinue>(Continue),
                EventStreams.UserInterface.Subscribe<EventPlay>(Play)
            };
        }

        private void Restart(EventRestart eventRestart)
        {
            EventStreams.UserInterface.Publish(new EventReleaseRunner());
            EventStreams.UserInterface.Publish(new EventHideScreenChoice());
            EventStreams.UserInterface.Publish(new EventHideFPSLocation());

            var currentRegion = _map.CurrentRegion;
            _stripGenerator.Generation(currentRegion.GetCurrentPartStrip());
            CheckToShowPanelNewReagent(currentRegion);
            _canvasGame.SetActive(true);
            ShowPlane(currentRegion);
        }

        private void OpenMap(EventOpenMap eventOpenMap)
        {
            EventStreams.UserInterface.Publish(new EventReleaseRunner());
            EventStreams.UserInterface.Publish(new EventHideScreenChoice());
            EventStreams.UserInterface.Publish(new EventHideFPSLocation());

            _canvasGame.SetActive(false);
        }

        private void Continue(EventContinue eventContinue)
        {
            EventStreams.UserInterface.Publish(new EventReleaseRunner());
            EventStreams.UserInterface.Publish(new EventHideScreenChoice());
            EventStreams.UserInterface.Publish(new EventHideFPSLocation());
            
            var currentRegion = _map.CurrentRegion;
            _stripGenerator.Generation(currentRegion.GetUnlockLevel());
            CheckToShowPanelNewReagent(currentRegion);
            _canvasGame.SetActive(true);
            ShowPlane(currentRegion);
        }

        private void Play(EventPlay eventPlay)
        {
            var currentRegion = _map.CurrentRegion;
            _stripGenerator.Generation(currentRegion.GetUnlockLevel());
            CheckToShowPanelNewReagent(currentRegion);
            _canvasGame.SetActive(true);
            ShowPlane(currentRegion);
        }

        private void ShowPlane(Region region)
        {
            foreach (var planeWorldType in _planeWorldTypes)
            {
                planeWorldType.Plane.SetActive(planeWorldType.WorldType == region.Information.WorldType);
            }
        }

        private void CheckToShowPanelNewReagent(Region region)
        {
            if (region.Information.AllLevelCompleted)
            {
                EventStreams.UserInterface.Publish(new EventHidePanelNewReagent());
            }
            else
            {
                EventStreams.UserInterface.Publish(new EventShowPanelNewReagent());
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        [Serializable]
        public class Settings
        {
            [FormerlySerializedAs("CanvasGame")]
            public GameObject Game;
            public List<PlaneWorldType> PlaneWorldTypes = new List<PlaneWorldType>();
        }

        [Serializable]
        public class PlaneWorldType
        {
            public GameObject Plane;
            public WorldType WorldType;
        }
    }
}