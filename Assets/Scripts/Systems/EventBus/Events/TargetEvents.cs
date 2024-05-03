public struct TargetDeathEvent : IEvent
{
	public Target Target;
}

public struct TargetDamageTakenEvent : IEvent
{
	public Target Target;
	public int Damage;
}

public struct TargetHealthChangedEvent : IEvent
{
	public Target Target;
	public int Health;
	public int MaxHealth;
}
