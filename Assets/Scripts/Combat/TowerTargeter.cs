using System.Collections.Generic;
using UnityEngine;

public static class TowerTargeter
{
	internal static Enemy[] GetEnemyTargets(float range)
	{
		var colliders = Physics.OverlapSphere(Tower.Instance.transform.position, range, LayerMask.GetMask("Enemy"));
		var enemies = new List<Enemy>();
		for (var i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].TryGetComponent<Enemy>(out var enemy))
			{
				if (!enemy.IsDead)
				{
					enemies.Add(enemy);
				}
			}
		}

		return enemies.ToArray();
	}

	static Enemy GetBestEnemy(Enemy[] enemies, string debuffName = null)
	{
		Enemy selectedTarget = null;
		var closestDistance = float.MaxValue;
		var fewestAttackers = int.MaxValue;
		var hasNonDebuffedEnemyBeenFound = false;

		foreach (var enemy in enemies)
		{
			var enemyHasDebuff = debuffName != null && enemy.HasDebuff(debuffName);
			var betterChoice = enemy.Attackers < fewestAttackers || (enemy.Attackers == fewestAttackers && enemy.DistanceToTower < closestDistance);

			if (!enemyHasDebuff && (!hasNonDebuffedEnemyBeenFound || betterChoice))
			{
				selectedTarget = enemy;
				fewestAttackers = enemy.Attackers;
				closestDistance = enemy.DistanceToTower;
				hasNonDebuffedEnemyBeenFound = true; // Now prioritizing non-debuffed enemies
			}
			else if (enemyHasDebuff && !hasNonDebuffedEnemyBeenFound && betterChoice)
			{
				selectedTarget = enemy;
				fewestAttackers = enemy.Attackers;
				closestDistance = enemy.DistanceToTower;
				// Don't update hasNonDebuffedEnemyBeenFound because we're still looking for non-debuffed targets.
			}
		}

		if (selectedTarget != null)
		{
			selectedTarget.Attackers++;
		}

		return selectedTarget;
	}

	static Enemy[] SortEnemies(Enemy[] enemies, int enemyCount, string debuffName = null)
	{
		var sortedEnemies = new List<Enemy>();
		var hasNonDebuffedEnemyBeenFound = false;

		for (var i = 0; i < enemyCount; i++)
		{
			Enemy bestEnemy = null;
			var closestDistance = float.MaxValue;
			var fewestAttackers = int.MaxValue;

			foreach (var enemy in enemies)
			{
				if (sortedEnemies.Contains(enemy))
				{
					continue;
				}

				var enemyHasDebuff = debuffName != null && enemy.HasDebuff(debuffName);
				var betterChoice = enemy.Attackers < fewestAttackers || (enemy.Attackers == fewestAttackers && enemy.DistanceToTower < closestDistance);

				if (!enemyHasDebuff && (!hasNonDebuffedEnemyBeenFound || betterChoice))
				{
					bestEnemy = enemy;
					fewestAttackers = enemy.Attackers;
					closestDistance = enemy.DistanceToTower;
					hasNonDebuffedEnemyBeenFound = true; // Now prioritizing non-debuffed enemies
				}
				else if (enemyHasDebuff && !hasNonDebuffedEnemyBeenFound && betterChoice)
				{
					bestEnemy = enemy;
					fewestAttackers = enemy.Attackers;
					closestDistance = enemy.DistanceToTower;
					// Don't update hasNonDebuffedEnemyBeenFound because we're still looking for non-debuffed targets.
				}
			}

			if (bestEnemy != null)
			{
				bestEnemy.Attackers++;
				sortedEnemies[i] = bestEnemy;
			}
		}

		return sortedEnemies.ToArray();
	}

	public static Enemy FindTargets(float range, string debuffName = null)
	{
		var enemies = GetEnemyTargets(range);
		var enemy = GetBestEnemy(enemies, debuffName);
		return enemy;
	}


	public static Enemy[] FindTargets(float range, int targetCount, string debuffName = null)
	{
		var enemies = GetEnemyTargets(range);
		var sortedEnemies = SortEnemies(enemies, targetCount, debuffName);
		return sortedEnemies;
	}
}
