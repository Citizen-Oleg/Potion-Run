using System;
using System.Collections.Generic;
using Events;
using FPS.EnemyComponent;
using Runner.GeneratorSystem;
using SaveAndLoadSystem;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FPS.GenerationSystem
{
    public class Generator : IDisposable
    {
        public int CurrentIndexLocation => _currentIndexLocation;
        
        private readonly StripGenerator _stripGenerator;
        private readonly List<Location> _locations;
        private readonly EnemyPool _enemyPool;
        
        private CompositeDisposable _subscription;
        private int _currentIndexLocation;
        private Location _activeLocation;

        private readonly Save _save;

        public Generator(Settings settings, StripGenerator stripGenerator, EnemyPool enemyPool, Save save, Load load)
        {
            _enemyPool = enemyPool;
            _stripGenerator = stripGenerator;
            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventGenerateShooter>(Generate),
                EventStreams.UserInterface.Subscribe<EventHideFPSLocation>(Hide)
                        
            };
            
            _locations = settings.Locations;
            
            _save = save;

            if (load.HasInformationIndexLocation())
            {
                _currentIndexLocation = load.GetIndexLocation();
            }
        }

        private void Hide(EventHideFPSLocation eventHideFpsLocation)
        {
            if (_activeLocation == null)
            {
                return;
            }
            
            _activeLocation.Camera.gameObject.SetActive(false);
            _activeLocation.gameObject.SetActive(false);
            
            foreach (var stripPoint in _activeLocation.StripPoint)
            {
                foreach (var stripPointEnemy in stripPoint.Enemies)
                {
                    _enemyPool.ReleaseEnemy(stripPointEnemy);
                }
                
                stripPoint.Enemies.Clear();
            }

            _enemyPool.ReleaseAll();
            _activeLocation = null;
        }

        private void Generate(EventGenerateShooter eventStartShooter)
        {
            if (_currentIndexLocation >= _locations.Count)
            {
                _currentIndexLocation = 0;
            }
            
            var index = _currentIndexLocation;

            _activeLocation = _locations[index];
            EventStreams.UserInterface.Publish(new EventCreateLocation(_activeLocation));
            _activeLocation.transform.position = _stripGenerator.Finish.RightPoint.position;
            _activeLocation.gameObject.SetActive(true);
            _activeLocation.NavigationBaker.Bake();

            foreach (var stripPoint in _activeLocation.StripPoint)
            {
                foreach (var enemyPoint in stripPoint.EnemyPoints)
                {
                    var randomEnemyType = enemyPoint.EnemyTypes[Random.Range(0, enemyPoint.EnemyTypes.Count)];
                    var enemy = _enemyPool.GetEnemyByType(randomEnemyType);

                    enemy.NavMeshAgent.gameObject.SetActive(false);
                    enemy.NavMeshAgent.transform.position = enemyPoint.Point.position;
                    enemy.NavMeshAgent.transform.rotation = Quaternion.LookRotation(enemyPoint.Point.forward);
                    enemy.NavMeshAgent.gameObject.SetActive(true);
                    
                    stripPoint.AddEnemy(enemy);
                }
            }

            _save.SaveIndexLocation(_currentIndexLocation);
            _currentIndexLocation++;
        }
        
        public void Dispose()
        {
            _subscription?.Dispose();
        }
        
        [Serializable]
        public class Settings
        {
            public List<Location> Locations = new List<Location>();
        }
    }
}