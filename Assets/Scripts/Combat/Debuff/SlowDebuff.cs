// public class SlowDebuff : IDebuff
// {
// 	public string Identifier { get; private set; }
// 	public float TotalDuration { get; private set; }
// 	private float remainingTime;
// 	public float SlowPercentage { get; private set; } // How much to slow the target by, expressed as a percentage

// 	public SlowDebuff(string identifier, float totalDuration, float slowPercentage)
// 	{
// 		Identifier = identifier;
// 		TotalDuration = totalDuration;
// 		SlowPercentage = slowPercentage;
// 		remainingTime = TotalDuration;
// 	}

// 	public void Apply(Target target)
// 	{
// 		// Apply the initial slow effect on the target
// 		target.AdjustSpeed(1 - SlowPercentage);
// 	}

// 	public void Tick(Target target)
// 	{
// 		// Called every frame/update, manages the duration of the slow
// 		if (remainingTime > 0)
// 		{
// 			remainingTime -= Time.deltaTime;
// 		}
// 		else
// 		{
// 			// Debuff duration has ended, reset target's speed and remove the debuff
// 			Remove(target);
// 		}
// 	}

// 	public void Refresh()
// 	{
// 		// Extend the total duration back to its initial value
// 		remainingTime = TotalDuration;
// 	}

// 	public void Remove(Target target)
// 	{
// 		// Reset target's speed to normal
// 		target.AdjustSpeed(1.0f / (1 - SlowPercentage));
// 	}
// }
