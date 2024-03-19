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
}
