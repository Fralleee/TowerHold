using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine<T>
{
	public event Action<IState<T>> OnTransition = delegate { };

	public IState<T> CurrentState;
	public float CurrentStateTime;

	readonly Dictionary<T, List<Transition<T>>> _transitions = new Dictionary<T, List<Transition<T>>>();
	List<Transition<T>> _currentTransitions = new List<Transition<T>>();

	static readonly List<Transition<T>> EmptyTransitions = new List<Transition<T>>(0);

	public void OnLogic()
	{
		CurrentStateTime += Time.deltaTime;
		var transition = GetTransition;
		if (transition != null)
		{
			SetState(transition.To);
		}

		CurrentState?.OnLogic();
	}

	public void SetState(IState<T> state)
	{
		if (state == CurrentState)
		{
			return;
		}

		CurrentState?.OnExit();
		CurrentState = state;

		_ = _transitions.TryGetValue(CurrentState.Identifier, out _currentTransitions);
		_currentTransitions ??= EmptyTransitions;

		CurrentState.OnEnter();
		CurrentStateTime = 0f;
		OnTransition(CurrentState);
	}

	public void AddTransition(IState<T> from, IState<T> to, Func<bool> predicate)
	{
		if (!_transitions.TryGetValue(from.Identifier, out var outTransitions))
		{
			outTransitions = new List<Transition<T>>();
			_transitions[from.Identifier] = outTransitions;
		}

		outTransitions.Add(new Transition<T>(to, predicate));
	}


	public void At(IState<T> to, IState<T> from, Func<bool> condition) => AddTransition(to, from, condition);

	Transition<T> GetTransition => _currentTransitions.FirstOrDefault(t => t.Condition());
}
