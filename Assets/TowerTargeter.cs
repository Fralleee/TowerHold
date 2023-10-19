using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting : MonoBehaviour, ITargeter
{
    public Health GetTarget(float targetingRange)
    {
        Enemy selectedTarget = null;
        float closestDistance = float.MaxValue;
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            /* For the enemy to be a valid target it has to be within the targetingRange
               We want to prioritize targets in the following order:

               - Are NOT targeted by others (enemy.IsTargeted = false)
               - By distance (closest to the tower)               
            */
            if (selectedTarget != null && !selectedTarget.IsTargeted && enemy.IsTargeted)
            {
                continue;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= targetingRange && distanceToEnemy < closestDistance)
            {
                selectedTarget = enemy;
                closestDistance = distanceToEnemy;
            }
        }

        if (selectedTarget)
        {
            selectedTarget.IsTargeted = true;
        }

        return selectedTarget?.health;
    }
}