using System;

public enum GameIcons
{
	Cooldown,
	Gold,
	Physical,
	Magical,
	Global,
}

public static class IconExtensions
{
	public static GameIcons AsIcon(this DamageType damageType)
	{
		return damageType switch
		{
			DamageType.Physical => GameIcons.Physical,
			DamageType.Magical => GameIcons.Magical,
			DamageType.Global => GameIcons.Global,
			_ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null),
		};
	}
}
