public struct EnemyDeathEvent : IEvent
{
	public Enemy Enemy;
}

public struct EnemySpawnEvent : IEvent
{
	public Enemy Enemy;
}

public struct EnemyVariantsSelectedEvent : IEvent
{
	public Enemy[] Variants;
}
