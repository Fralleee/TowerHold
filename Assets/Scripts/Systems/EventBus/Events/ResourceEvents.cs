public struct ResourceChangedEvent : IEvent
{
	public int CurrentResources;
}

public struct IncomeChangedEvent : IEvent
{
	public int CurrentIncome;
}
