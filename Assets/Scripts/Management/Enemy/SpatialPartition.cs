using System.Collections.Generic;

public static class SpatialPartition
{
	public static readonly Dictionary<SpatialZone, float> Zones = new Dictionary<SpatialZone, float>
  {
	{ SpatialZone.Melee, 6f },
	{ SpatialZone.Short, 16f },
	{ SpatialZone.Medium, 24f },
	{ SpatialZone.Long, 32f },
	{ SpatialZone.VeryLong, 40f }
  };

	public static SpatialZone DetermineZone(float distance)
	{
		return distance switch
		{
			_ when distance <= Zones[SpatialZone.Melee] => SpatialZone.Melee,
			_ when distance <= Zones[SpatialZone.Short] => SpatialZone.Short,
			_ when distance <= Zones[SpatialZone.Medium] => SpatialZone.Medium,
			_ when distance <= Zones[SpatialZone.Long] => SpatialZone.Long,
			_ when distance <= Zones[SpatialZone.VeryLong] => SpatialZone.VeryLong,
			_ => SpatialZone.OutOfRange,
		};
	}

	public static float GetRange(this AttackRange attackRange) => Zones.TryGetValue(attackRange.ToZone(), out var range) ? range : 0f;


	public static float DistanceToNextZone(this SpatialZone zone, float distanceToTower)
	{
		if (zone == SpatialZone.Melee)
		{
			return 0;
		}

		var nextZone = zone - 1;
		var distanceToNextZone = distanceToTower - Zones[nextZone];

		return distanceToNextZone;
	}

	public static SpatialZone ToZone(this AttackRange attackRange) => (SpatialZone)attackRange;

}
