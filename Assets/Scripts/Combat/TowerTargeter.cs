public static class TowerTargeter
{
	public static Enemy GetEnemyTarget(AttackRange attackRange, string debuffName = null)
	{
		Enemy selectedTarget = null;
		var closestDistance = float.MaxValue;
		var fewestAttackers = int.MaxValue;
		var isTargetWithoutDebuffPreferred = false;

		var enemies = EnemyManager.SpatialPartitionManager.GetEnemiesWithinZone(attackRange.ToZone());
		foreach (var enemy in enemies)
		{
			var enemyHasDebuff = debuffName != null && enemy.ActiveDebuffs.ContainsKey(debuffName);
			var betterChoice = enemy.Attackers < fewestAttackers || (enemy.Attackers == fewestAttackers && enemy.DistanceToTower < closestDistance);

			// Update conditions to consider debuff presence
			if ((selectedTarget == null || betterChoice) && (!enemyHasDebuff || !isTargetWithoutDebuffPreferred))
			{
				selectedTarget = enemy;
				fewestAttackers = enemy.Attackers;
				closestDistance = enemy.DistanceToTower;
				isTargetWithoutDebuffPreferred = !enemyHasDebuff;
			}
		}

		if (selectedTarget != null)
		{
			selectedTarget.Attackers++;
		}

		return selectedTarget;
	}
}
