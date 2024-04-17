public class DamageBuildUpDebuff : IDebuff
{
	public string Identifier { get; private set; }
	public float MultiplierIncrease { get; private set; }
	public int MaxStacks { get; private set; }

	readonly float _maxMultiplier;
	Target _target;

	public DamageBuildUpDebuff(string identifier, float multiplierIncrease, int maxStacks)
	{
		Identifier = identifier;
		MultiplierIncrease = multiplierIncrease;
		MaxStacks = maxStacks;
		_maxMultiplier = maxStacks * multiplierIncrease;
	}

	public void Apply(Target target)
	{
		_target = target;
		target.DamageModifiers.AddModifier(Identifier, MultiplierIncrease, _maxMultiplier);
	}

	public void Tick(Target target)
	{
		// This debuff doesn't do anything on tick, it's purely a marker for increased damage
	}

	public void Refresh()
	{
		_target.DamageModifiers.AddModifier(Identifier, MultiplierIncrease, _maxMultiplier);
	}

	public void Remove(Target target)
	{
		target.RemoveDebuff(this);
	}
}
