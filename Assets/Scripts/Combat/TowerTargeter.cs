public static class TowerTargeter
{
	public static Enemy GetEnemyTarget(AttackRange attackRange, string debuffName = null)
	{
		Enemy selectedTarget = null;
		var closestDistance = float.MaxValue;
		var fewestAttackers = int.MaxValue;
		var hasNonDebuffedEnemyBeenFound = false;

		var enemies = EnemyManager.SpatialPartitionManager.GetEnemiesWithinZone(attackRange.ToZone());
		foreach (var enemy in enemies)
		{
			var enemyHasDebuff = debuffName != null && enemy.ActiveDebuffs.ContainsKey(debuffName);
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
}
