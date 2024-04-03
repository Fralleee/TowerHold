// public class StunDebuff : IDebuff
// {
// 	public string Identifier { get; private set; }
// 	public float TotalDuration { get; private set; }
// 	private float remainingTime;
// 	private bool isApplied;

// 	public StunDebuff(string identifier, float totalDuration)
// 	{
// 		Identifier = identifier;
// 		TotalDuration = totalDuration;
// 		remainingTime = TotalDuration;
// 	}

// 	public void Apply(Target target)
// 	{
// 		// Apply the stun effect only if it's not already applied
// 		if (!isApplied)
// 		{
// 			target.DisableActions(); // Assume this method disables the target's ability to move or act
// 			isApplied = true;
// 		}
// 	}

// 	public void Tick(Target target)
// 	{
// 		// Called every frame/update, manages the duration of the stun
// 		if (remainingTime > 0)
// 		{
// 			remainingTime -= Time.deltaTime;
// 		}
// 		else if (isApplied)
// 		{
// 			// Stun duration has ended, re-enable target's actions and remove the debuff
// 			Remove(target);
// 		}
// 	}

// 	public void Refresh()
// 	{
// 		// Extend the stun duration back to its initial value without resetting the tick timer
// 		remainingTime = TotalDuration;
// 	}

// 	public void Remove(Target target)
// 	{
// 		// Re-enable target's actions
// 		target.EnableActions(); // Assume this method re-enables the target's ability to move or act
// 		isApplied = false;
// 	}
// }
