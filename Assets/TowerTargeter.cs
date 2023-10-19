using UnityEngine;

public static class Targeter
{
    public static Health GetTurretTarget(Transform turret, float range)
    {
        Enemy selectedTarget = null;
        float closestDistance = float.MaxValue;
        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            /* For the enemy to be a valid target it has to be within the range
               We want to prioritize targets in the following order:

               - Are NOT targeted by others (enemy.IsTargeted = false)
               - By distance (closest to the tower)               
            */
            if (selectedTarget != null && !selectedTarget.IsTargeted && enemy.IsTargeted)
            {
                continue;
            }

            float distanceToEnemy = Vector3.Distance(turret.position, enemy.transform.position);
            if (distanceToEnemy <= range && distanceToEnemy < closestDistance)
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