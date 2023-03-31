using System;
using Base.SimpleEventBus_and_MonoPool;
using GapBetweenRunnerAndShooter.CraftSystem;
using UnityEngine;

namespace FPS.AttackSystem
{
    public class ProjectileFactory
    {
        private readonly DefaultMonoBehaviourPool<Projectile> _pool;

        private readonly CraftController _craftController;
        private readonly ModelColorProvider _modelColorProvider;

        public ProjectileFactory(Settings settings, Transform parent, CraftController craftController, ModelColorProvider modelColorProvider)
        {
            _craftController = craftController;
            _modelColorProvider = modelColorProvider;
            _pool = new DefaultMonoBehaviourPool<Projectile>(settings.ProjectilePrefab, parent, settings.PoolSize);
        }

        public Projectile GetProjectile()
        {
            var projectile = _pool.Take();
            projectile.LiquidVolume.LiquidColor1 =
                _modelColorProvider.GetColorByReagentType(_craftController.CurrentModelType);
            projectile.Initialize(this);
            return projectile;
        }

        public void Refresh(Projectile projectile)
        {
            projectile.LiquidVolume.LiquidColor1 =
                _modelColorProvider.GetColorByReagentType(_craftController.CurrentModelType);
        }

        public void Release(Projectile projectile)
        {
            _pool.Release(projectile);
        }

        public void ResetParent(Projectile projectile)
        {
            _pool.ResetParent(projectile);
        }

        [Serializable]
        public class Settings
        {
            public int PoolSize;
            public Projectile ProjectilePrefab;
        }
    }
}