﻿using System;
using System.Collections.Generic;
using Base.SimpleEventBus_and_MonoPool;
using Events;
using GapBetweenRunnerAndShooter.CraftSystem;
using ParticleFactory;
using Tools.SimpleEventBus;
using UniRx;
using UnityEngine;

namespace FPS.EnemyComponent
{
    public class MonsterSpawner : IDisposable
    {
        private readonly Dictionary<ModelType, DefaultMonoBehaviourPool<Monster>> _monster =
            new Dictionary<ModelType, DefaultMonoBehaviourPool<Monster>>();

        private readonly CompositeDisposable _subscription;
        private readonly CraftController _craftController;
        private readonly ModelColorProvider _modelColorProvider;

        public MonsterSpawner(Settings settings, Transform transform, CraftController craftController,
            ModelColorProvider modelColorProvider)
        {
            _craftController = craftController;
            _modelColorProvider = modelColorProvider;

            foreach (var settingsMonsterType in settings.MonsterTypes)
            {
                var pool = new DefaultMonoBehaviourPool<Monster>(settingsMonsterType.Monster, transform, settings.SizePool);
                _monster.Add(settingsMonsterType.ModelType, pool);
            }

            _subscription = new CompositeDisposable
            {
                EventStreams.UserInterface.Subscribe<EventEnemyDead>(CreateMonster),
                EventStreams.UserInterface.Subscribe<EventGenerationRunner>(Event =>
                {
                    foreach (var defaultMonoBehaviourPool in _monster)
                    {
                        defaultMonoBehaviourPool.Value.ReleaseAll();
                    }
                })
            };
        }

        private void CreateMonster(EventEnemyDead eventEnemyDead)
        {
            var enemy = eventEnemyDead.Enemy;
            var context = new ContextSpawnParticle(_modelColorProvider.GetColorByReagentTypeToParticle(_craftController.CurrentModelType));
            var eventParticle = new EventShowContextParticle(enemy.FxPosition, ParticleType.SpawnMonster, context);
            EventStreams.UserInterface.Publish(eventParticle);
            
            var monsterPool = _monster[_craftController.CurrentModelType];
            var monster = monsterPool.Take();

            enemy.CurrentMonster = monster;
            
            monster.NavMeshAgent.gameObject.SetActive(false);
            monster.NavMeshAgent.transform.position = enemy.transform.position;
            monster.NavMeshAgent.transform.rotation = enemy.transform.rotation;
            monster.NavMeshAgent.gameObject.SetActive(true);

            monster.Initialize(enemy.StripPoint, this);
        }

        public Monster GetMonster()
        {
            var monsterPool = _monster[_craftController.CurrentModelType];
            return  monsterPool.Take();
        }

        public void Release(Monster monster)
        {
            var context = new ContextSpawnParticle(_modelColorProvider.GetColorByReagentTypeToParticle(_craftController.CurrentModelType));
            var eventParticle = new EventShowContextParticle(monster.FxPosition, ParticleType.SpawnMonster, context);
            EventStreams.UserInterface.Publish(eventParticle);
            _monster[_craftController.CurrentModelType].Release(monster);
        }
        
        public void Dispose()
        {
            _subscription?.Dispose();
        }
        
        [Serializable]
        public class Settings
        {
            public int SizePool;
            public List<MonsterType> MonsterTypes = new List<MonsterType>();
        }

        [Serializable]
        public class MonsterType
        {
            public ModelType ModelType;
            public Monster Monster;
        }
    }
}