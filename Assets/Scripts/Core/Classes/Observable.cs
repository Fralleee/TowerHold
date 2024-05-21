using System;
using System.Collections.Generic;

[Serializable]
public class Observable<T>
{
	T _value;
	public event Action<T> ValueChanged;

	public T Value
	{
		get
		{
			return _value;
		}

		set
		{
			Set(value);
		}
	}

	public static implicit operator T(Observable<T> observable)
	{
		return observable._value;
	}

	public Observable(T value, Action<T> onValueChanged = null)
	{
		_value = value;

		if (onValueChanged != null)
		{
			ValueChanged += onValueChanged;
		}
	}

	public void Set(T value)
	{
		if (EqualityComparer<T>.Default.Equals(_value, value))
		{
			return;
		}

		_value = value;
		Invoke();
	}

	public void Invoke()
	{
		ValueChanged?.Invoke(_value);
	}

	public void AddListener(Action<T> handler)
	{
		ValueChanged += handler;
	}

	public void RemoveListener(Action<T> handler)
	{
		ValueChanged -= handler;
	}

	public void Dispose()
	{
		ValueChanged = null;
		_value = default;
	}
}
