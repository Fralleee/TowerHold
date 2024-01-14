using System;
using UnityEngine;

public class RandomGenerator
{
	readonly System.Random _random;

	public RandomGenerator(int seed)
	{
		_random = new System.Random(seed);
	}

	public int Next(int minValue, int maxValue)
	{
		return _random.Next(minValue, maxValue);
	}

	public float NextFloat()
	{
		return (float)_random.NextDouble();
	}

	public float NextFloat(float minValue, float maxValue)
	{
		return (float)((_random.NextDouble() * (maxValue - minValue)) + minValue);
	}

	public float Variance(float value)
	{
		return value + (float)((_random.NextDouble() * 0.1 * 2) - 0.1);
	}

	public Vector2 InsideUnitCircleNormalized()
	{
		var angle = _random.NextDouble() * Math.PI * 2;
		var radius = Math.Sqrt(_random.NextDouble());

		var x = (float)(radius * Math.Cos(angle));
		var y = (float)(radius * Math.Sin(angle));

		var point = new Vector2(x, y);
		return point.normalized;
	}
}
