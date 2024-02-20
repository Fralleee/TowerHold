using System.Collections.Generic;

public class SpatialPartitionManager
{
	readonly Dictionary<SpatialZone, List<Enemy>> _enemyZones = new Dictionary<SpatialZone, List<Enemy>>();

	public SpatialPartitionManager()
	{
		foreach (SpatialZone range in System.Enum.GetValues(typeof(SpatialZone)))
		{
			_enemyZones[range] = new List<Enemy>();
		}
	}

	public void RemoveEnemy(Enemy enemy)
	{
		_ = _enemyZones[enemy.CurrentZone].Remove(enemy);
	}

	public (SpatialZone currentZone, float distanceToNextZone) UpdateZone(Enemy enemy)
	{
		var zone = SpatialPartition.DetermineZone(enemy.DistanceToTower - enemy.DistanceCoveredBeforeNextCheck);
		if (zone == enemy.CurrentZone)
		{
			return (zone, SpatialPartition.DistanceToNextZone(zone, enemy.DistanceToTower));
		}

		_enemyZones[enemy.CurrentZone].Remove(enemy);
		_enemyZones[zone].Add(enemy);

		return (zone, SpatialPartition.DistanceToNextZone(zone, enemy.DistanceToTower));
	}

	public List<Enemy> GetEnemiesWithinZone(SpatialZone attackZone)
	{
		var enemiesInRange = new List<Enemy>();
		foreach (SpatialZone zone in System.Enum.GetValues(typeof(SpatialZone)))
		{
			if (zone <= attackZone && zone != SpatialZone.OutOfRange)
			{
				enemiesInRange.AddRange(_enemyZones[zone]);
			}
		}

		return enemiesInRange;
	}
}
