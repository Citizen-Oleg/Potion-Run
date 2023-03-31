using System.Collections.Generic;
using UnityEngine;

namespace FPS.EnemyComponent
{
    public static class NearestTargetProvider
    {
        public static Enemy GetNearestTarget(List<Enemy> enemies, Vector3 position)
        {
            if (enemies.Count == 0)
            {
                return null;
            }

            var minDistance = float.MaxValue;
            var minIndex = 0;

            for (var index = 0; index < enemies.Count; index++)
            {
                var target = enemies[index];

                if (target.IsDead)
                {
                    continue;
                }

                var distanceToTarget = Vector3.Distance(target.transform.position, position);

                if (minDistance > distanceToTarget)
                {
                    minDistance = distanceToTarget;
                    minIndex = index;
                }
            }

            return enemies[minIndex].IsDead ? null : enemies[minIndex];
        }

        public static Transform GetNearestPoint(List<Transform> points, Vector3 position)
        {
            if (points.Count == 0)
            {
                return null;
            }

            var minDistance = float.MaxValue;
            var minIndex = 0;

            for (var index = 0; index < points.Count; index++)
            {
                var target = points[index];
                
                var distanceToTarget = Vector3.Distance(target.transform.position, position);

                if (minDistance > distanceToTarget)
                {
                    minDistance = distanceToTarget;
                    minIndex = index;
                }
            }

            return points[minIndex];
        }
    }
}