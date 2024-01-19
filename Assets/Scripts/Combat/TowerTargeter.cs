using UnityEngine;

public static class TowerTargeter
{
	public static Enemy GetEnemyTarget(Transform turret, float range)
	{
		Enemy selectedTarget = null;
		var closestDistance = float.MaxValue;
		var fewestAttackers = int.MaxValue; // This holds the smallest number of attackers found

		// Loop through all enemies to find the one with fewest attackers and within the closest distance
		foreach (var enemy in Enemy.AllEnemies)
		{
			/* For the enemy to be a valid target it has to be within the range
               We want to prioritize targets in the following order:

               - By number of attackers (fewer, the better)
               - By distance (closest to the tower)
            */
			var distanceToEnemy = Vector3.Distance(turret.position, enemy.transform.position);
			if (distanceToEnemy > range)
			{
				continue;
			}

			// Check if this enemy has fewer attackers or is closer while having the same number of attackers
			if (enemy.Attackers < fewestAttackers || (enemy.Attackers == fewestAttackers && distanceToEnemy < closestDistance))
			{
				selectedTarget = enemy;
				fewestAttackers = enemy.Attackers;
				closestDistance = distanceToEnemy;
			}
		}
		// Increment the attackers count for the selected target
		if (selectedTarget != null)
		{
			selectedTarget.Attackers++;
		}

		return selectedTarget;
	}
}
