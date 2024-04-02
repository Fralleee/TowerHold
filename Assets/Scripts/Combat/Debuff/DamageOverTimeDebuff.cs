
public class DamageOverTimeDebuff : IDebuff
{
    public string Identifier { get; private set; }
    public float TotalDuration { get; private set; } = 10f; // Total duration of the debuff
    public float TotalDamage { get; private set; } = 300f; // Total damage the debuff should deal
    public float TickRate { get; private set; } = 1f; // How often the debuff applies damage
    float nextTickTime; // When the next tick should occur
    float remainingTime; // Remaining time for the debuff
    float damagePerTick; // Damage dealt each tick

    public DamageOverTime(string identifier, float totalTime, float totalDamage, float tickRate)
    {
        Identifier = identifier;
        TotalDuration = totalDuration;
        TotalDamage = totalDamage;
        TickRate = tickRate;
        nextTickTime = TickRate;
        remainingTime = TotalDuration;
        damagePerTick = TotalDamage / (TotalDuration / TickRate);
    }

    public void Apply(Target target)
    {
        // Initial application logic here, if any
    }

    public void Tick(Target target)
    {
 		if(Time.time >= nextTickTime)
        {
            target.TakeDamage(damagePerTick);
            nextTickTime += TickRate;
            remainingTime -= TickRate;
        }

        if (remainingTime <= 0)
        {
            End(target);
        }
    }

	public void Refresh()
	{
        // Extend the total duration without resetting the tick timer
        // Calculate the new remaining time based on the extension
        remainingTime = TotalDuration;

        // Adjust the next tick time if necessary (it might not be if you're just extending the duration without altering the tick rate or the timing of the next tick)
        // nextTickTime remains unchanged to avoid resetting the tick timer
	}

    public void End(Target target)
    {
        target.RemoveDebuff(this);
    }
}
