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

	// Adds a random variance to a value between -0.1f and 0.1f
	public float Variance(float value)
	{
		return value + (float)((_random.NextDouble() * 0.1 * 2) - 0.1);
	}

	public Vector2 InsideUnitCircle()
	{
		var angle = _random.NextDouble() * Math.PI * 2;
		var radius = Math.Sqrt(_random.NextDouble());

		var x = (float)(radius * Math.Cos(angle));
		var y = (float)(radius * Math.Sin(angle));

		var point = new Vector2(x, y);
		return point;
	}

	public Vector3 InsideUnitSphere()
	{
		var u = _random.NextDouble();
		var v = _random.NextDouble();
		var theta = u * 2.0 * Math.PI;
		var phi = Math.Acos((2.0 * v) - 1.0);
		var r = Math.Pow(_random.NextDouble(), 1.0 / 3.0); // Cube root for uniform distribution

		var x = r * Math.Sin(phi) * Math.Cos(theta);
		var y = r * Math.Sin(phi) * Math.Sin(theta);
		var z = r * Math.Cos(phi);

		return new Vector3((float)x, (float)y, (float)z);
	}

}
