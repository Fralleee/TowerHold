
using UnityEngine;

public class DamageOverTimeDebuff : IDebuff
{
	public string Identifier { get; private set; }
	public float TotalDuration { get; private set; } = 10f; // Total duration of the debuff
	public float TotalDamage { get; private set; } = 300f; // Total damage the debuff should deal
	public float TickRate { get; private set; } = 1f; // How often the debuff applies damage
	float _nextTickTime; // When the next tick should occur
	float _remainingTime; // Remaining time for the debuff
	readonly float _damagePerTick; // Damage dealt each tick

	public DamageOverTimeDebuff(string identifier, float totalDuration, float totalDamage, float tickRate)
	{
		Identifier = identifier;
		TotalDuration = totalDuration;
		TotalDamage = totalDamage;
		TickRate = tickRate;
		_nextTickTime = TickRate;
		_remainingTime = TotalDuration;
		_damagePerTick = TotalDamage / (TotalDuration / TickRate);
	}

	public void Apply(Target target)
	{
		// Initial application logic here, if any
	}

	public void Tick(Target target)
	{
		if (Time.time >= _nextTickTime)
		{
			Debug.Log($"Dealing {_damagePerTick} damage to " + target.name);
			_ = target.TakeDamage((int)_damagePerTick);
			_nextTickTime += TickRate;
			_remainingTime -= TickRate;
		}

		if (_remainingTime <= 0)
		{
			Remove(target);
		}
	}

	public void Refresh()
	{
		// Extend the total duration without resetting the tick timer
		// Calculate the new remaining time based on the extension
		_remainingTime = TotalDuration;

		// Adjust the next tick time if necessary (it might not be if you're just extending the duration without altering the tick rate or the timing of the next tick)
		// nextTickTime remains unchanged to avoid resetting the tick timer
	}

	public void Remove(Target target)
	{
		target.RemoveDebuff(this);
	}
}
