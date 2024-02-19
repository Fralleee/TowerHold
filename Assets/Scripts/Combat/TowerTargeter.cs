public static class TowerTargeter
{
	public static Enemy GetEnemyTarget(AttackRange attackRange)
	{
		Enemy selectedTarget = null;
		var closestDistance = float.MaxValue;
		var fewestAttackers = int.MaxValue;
		var enemies = EnemyManager.SpatialPartitionManager.GetEnemiesWithinZone(attackRange.ToZone());
		foreach (var enemy in enemies)
		{
			// For the enemy to be a valid target it has to be within the range
			// We want to prioritize targets in the following order:
			// - By number of attackers (fewer, the better)
			// - By distance (closest to the tower)
			// Check if this enemy has fewer attackers or is closer while having the same number of attackers
			if (enemy.Attackers < fewestAttackers || (enemy.Attackers == fewestAttackers && enemy.DistanceToTower < closestDistance))
			{
				selectedTarget = enemy;
				fewestAttackers = enemy.Attackers;
				closestDistance = enemy.DistanceToTower;
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
