// public class SilenceDebuff : IDebuff
// {
// 	public string Identifier { get; private set; }
// 	public float TotalDuration { get; private set; }
// 	private float remainingTime;
// 	private bool isApplied;

// 	public SilenceDebuff(string identifier, float totalDuration)
// 	{
// 		Identifier = identifier;
// 		TotalDuration = totalDuration;
// 		remainingTime = TotalDuration;
// 	}

// 	public void Apply(Target target)
// 	{
// 		// Apply the silence effect only if it's not already applied
// 		if (!isApplied)
// 		{
// 			target.DisableAbilities(); // Assume this method disables the target's special abilities or spells
// 			isApplied = true;
// 		}
// 	}

// 	public void Tick(Target target)
// 	{
// 		// Called every frame/update, manages the duration of the silence
// 		if (remainingTime > 0)
// 		{
// 			remainingTime -= Time.deltaTime;
// 		}
// 		else if (isApplied)
// 		{
// 			// Silence duration has ended, re-enable target's abilities and remove the debuff
// 			Remove(target);
// 		}
// 	}

// 	public void Refresh()
// 	{
// 		// Extend the silence duration back to its initial value without resetting the tick timer
// 		remainingTime = TotalDuration;
// 	}

// 	public void Remove(Target target)
// 	{
// 		// Re-enable target's special abilities
// 		target.EnableAbilities(); // Assume this method re-enables the target's abilities
// 		isApplied = false;
// 	}
// }
