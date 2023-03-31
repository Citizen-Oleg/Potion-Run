using System;
using System.Collections.Generic;
using Base.SimpleEventBus_and_MonoPool;
using UnityEngine;

namespace FPS.EnemyComponent
{
    public class EnemyPool
    {
        private readonly Dictionary<EnemyType, DefaultMonoBehaviourPool<Enemy>> _enemyPools =
            new Dictionary<EnemyType, DefaultMonoBehaviourPool<Enemy>>();
         
        public EnemyPool(Settings settings, Transform parent)
        {
            foreach (var enemy in settings.Enemies)
            {
                var pool = new DefaultMonoBehaviourPool<Enemy>(enemy, parent, settings.PoolSize);
                _enemyPools.Add(enemy.EnemyType, pool);
            }
        }

        public Enemy GetEnemyByType(EnemyType enemyType)
        {
            var enemy = _enemyPools[enemyType].Take();
            enemy.Initialize(this);
            return enemy;
        }

        public void ReleaseEnemy(Enemy enemy)
        {
            _enemyPools[enemy.EnemyType].Release(enemy);
        }

        public void ReleaseAll()
        {
            foreach (var defaultMonoBehaviourPool in _enemyPools)
            {
                defaultMonoBehaviourPool.Value.ReleaseAll();
            }
        }
        
        [Serializable]
        public class Settings
        {
            public int PoolSize;
            public List<Enemy> Enemies = new List<Enemy>();
        }
    }
}