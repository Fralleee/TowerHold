using System;

public class Transition<T>
{
	public Func<bool> Condition { get; }
	public IState<T> To { get; }

	public Transition(IState<T> to, Func<bool> condition)
	{
		To = to;
		Condition = condition;
	}
}
