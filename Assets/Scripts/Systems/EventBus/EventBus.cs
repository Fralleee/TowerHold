using System.Collections.Generic;

public static class EventBus<T> where T : IEvent
{
	static readonly HashSet<IEventBinding<T>> Bindings = new HashSet<IEventBinding<T>>();

	public static void Register(EventBinding<T> binding) => Bindings.Add(binding);
	public static void Deregister(EventBinding<T> binding) => Bindings.Remove(binding);

	public static void Raise(T @event)
	{
		foreach (var binding in Bindings)
		{
			binding.OnEvent.Invoke(@event);
			binding.OnEventNoArgs.Invoke();
		}
	}

#pragma warning disable IDE0051 // Remove unused private members
	static void Clear()
#pragma warning restore IDE0051 // Remove unused private members
	{
		Bindings.Clear();
	}
}
