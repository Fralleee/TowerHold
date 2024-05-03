public struct TowerDeathEvent : IEvent
{
}

public struct TowerHealthChangedEvent : IEvent
{
	public int Health;
	public int MaxHealth;
}
