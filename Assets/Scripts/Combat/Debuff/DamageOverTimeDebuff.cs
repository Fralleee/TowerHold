
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
	GameObject _debuffEffect;
	DamageType _damageType;

	public DamageOverTimeDebuff(string identifier, float totalDuration, float totalDamage, float tickRate, GameObject effect, DamageType damageType)
	{
		Identifier = identifier;
		TotalDuration = totalDuration;
		TotalDamage = totalDamage;
		TickRate = tickRate;
		_nextTickTime = Time.time + TickRate;
		_remainingTime = TotalDuration;
		_damagePerTick = TotalDamage / (TotalDuration / TickRate);
		_debuffEffect = effect;
		_damageType = damageType;
	}

	public void Apply(Target target)
	{
		if (!_debuffEffect)
		{
			return;
		}

		_debuffEffect = Object.Instantiate(_debuffEffect, target.Center.position, Quaternion.identity, target.transform);
		var particleSystems = _debuffEffect.GetComponentsInChildren<ParticleSystem>();
		foreach (var particleSystem in particleSystems)
		{
			var main = particleSystem.main;
			main.startLifetime = TickRate;
		}
	}

	public void Tick(Target target)
	{
		if (Time.time >= _nextTickTime)
		{
			_ = target.TakeDamage((int)_damagePerTick, _damageType);
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
		Object.Destroy(_debuffEffect);
	}
}
