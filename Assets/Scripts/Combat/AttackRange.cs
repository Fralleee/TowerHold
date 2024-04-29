public enum AttackRange
{
	Melee,
	Short,
	Medium,
	Long,
	VeryLong
}

public static class AttackRangeExtensions
{
	public static string AsText(this AttackRange attackRange)
	{
		return attackRange switch
		{
			AttackRange.Melee => "Melee range",
			AttackRange.Short => "Short range",
			AttackRange.Medium => "Medium range",
			AttackRange.Long => "Long range",
			AttackRange.VeryLong => "Very long range",
			_ => "N/A",
		};
	}

	public static float GetRange(this AttackRange attackRange)
	{
		return attackRange switch
		{
			AttackRange.Melee => 6f,
			AttackRange.Short => 16f,
			AttackRange.Medium => 24f,
			AttackRange.Long => 32f,
			AttackRange.VeryLong => 40f,
			_ => 0f,
		};
	}
}
