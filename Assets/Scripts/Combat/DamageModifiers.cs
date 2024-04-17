using System.Collections.Generic;

public class DamageModifiers
{
	public Dictionary<string, float> Modifiers;

	public DamageModifiers()
	{
		Modifiers = new Dictionary<string, float>();
	}

	public void AddModifier(string identifier, float multiplier, float maxMultiplier)
	{
		if (Modifiers.ContainsKey(identifier))
		{
			if (Modifiers[identifier] + multiplier > maxMultiplier)
			{
				Modifiers[identifier] = maxMultiplier;
			}
			else
			{
				Modifiers[identifier] += multiplier;
			}
		}
		else
		{
			Modifiers.Add(identifier, multiplier);
		}
	}

	public void RemoveModifier(string identifier)
	{
		if (Modifiers.ContainsKey(identifier))
		{
			_ = Modifiers.Remove(identifier);
		}
	}

	public float GetMultiplier()
	{
		float multiplier = 1;
		foreach (var modifier in Modifiers.Values)
		{
			multiplier += modifier;
		}
		return multiplier;
	}
}
