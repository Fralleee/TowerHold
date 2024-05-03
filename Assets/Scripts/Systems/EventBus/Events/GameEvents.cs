public struct GameStartEvent : IEvent
{
}

public struct GameEndEvent : IEvent
{
}

public struct PreparationCompleteEvent : IEvent
{
}

public struct LevelChangedEvent : IEvent
{
	public int CurrentLevel;
}
