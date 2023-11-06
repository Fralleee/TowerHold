using System.Linq;
using UnityEngine;

public static class TowerTargeter
{

    public static Health GetTurretTarget(Transform turret, float range)
    {
        Enemy selectedTarget = null;
        float closestDistance = float.MaxValue;
        int fewestAttackers = int.MaxValue; // This holds the smallest number of attackers found

        // Loop through all enemies to find the one with fewest attackers and within the closest distance
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            /* For the enemy to be a valid target it has to be within the range
               We want to prioritize targets in the following order:

               - By number of attackers (fewer, the better)
               - By distance (closest to the tower)               
            */
            float distanceToEnemy = Vector3.Distance(turret.position, enemy.transform.position);
            if (distanceToEnemy > range)
            {
                continue;
            }

            // Check if this enemy has fewer attackers or is closer while having the same number of attackers
            if (enemy.attackers < fewestAttackers || (enemy.attackers == fewestAttackers && distanceToEnemy < closestDistance))
            {
                selectedTarget = enemy;
                fewestAttackers = enemy.attackers;
                closestDistance = distanceToEnemy;
            }
        }
        // Increment the attackers count for the selected target
        if (selectedTarget != null)
        {
            selectedTarget.attackers++;
        }

        return selectedTarget;
    }

    // public static Health GetTurretTarget_OLD(Transform turret, float range)
    // {
    //     Enemy selectedTarget = null;
    //     float closestDistance = float.MaxValue;
    //     foreach (Enemy enemy in Enemy.AllEnemies)
    //     {
    //         /* For the enemy to be a valid target it has to be within the range
    //            We want to prioritize targets in the following order:

    //            - Are NOT targeted by others (enemy.IsTargeted = false)
    //            - By distance (closest to the tower)               
    //         */
    //         if (selectedTarget != null && !selectedTarget.HasAttackers && enemy.HasAttackers)
    //         {
    //             continue;
    //         }

    //         float distanceToEnemy = Vector3.Distance(turret.position, enemy.transform.position);
    //         if (distanceToEnemy <= range && distanceToEnemy < closestDistance)
    //         {
    //             selectedTarget = enemy;
    //             closestDistance = distanceToEnemy;
    //         }
    //     }

    //     if (selectedTarget)
    //     {
    //         selectedTarget.attackers += 1;
    //     }

    //     return selectedTarget;
    // }
}