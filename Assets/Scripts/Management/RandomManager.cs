
using System;
using UnityEngine;

public static class RandomManager
{
	static System.Random _shopRandom;
	static System.Random _enemyRandom;
	static System.Random _delayRandom;
	static System.Random _unSeededRandom = new System.Random();

	public static void InitializeWithSeed(int seed)
	{
		_shopRandom = new System.Random(seed);
		_enemyRandom = new System.Random(seed);
		_delayRandom = new System.Random(seed);
	}

	public static void SetSeed(int startSeed, int currentLevel)
	{
		var hash = HashCode.Combine(startSeed, currentLevel);
		InitializeWithSeed(hash);
	}

	public static int Shop(int minValue, int maxValue)
	{
		return _shopRandom.Next(minValue, maxValue);
	}

	public static int Enemy(int minValue, int maxValue)
	{
		return _enemyRandom.Next(minValue, maxValue);
	}

	public static float Delay(float minValue, float maxValue)
	{
		return (float)((_delayRandom.NextDouble() * (maxValue - minValue)) + minValue);
	}

	public static float DelayUnseed(float minValue, float maxValue)
	{
		return (float)((_unSeededRandom.NextDouble() * (maxValue - minValue)) + minValue);
	}

	public static float RandomDelay(float value)
	{
		return value + (float)((_delayRandom.NextDouble() * 0.1 * 2) - 0.1);
	}

	public static Vector2 InsideUnitCircleNormalized()
	{
		var angle = NextDouble() * Math.PI * 2; // Random angle
		var radius = Math.Sqrt(NextDouble()); // Random radius, square root for uniform distribution

		var x = (float)(radius * Math.Cos(angle));
		var y = (float)(radius * Math.Sin(angle));

		var point = new Vector2(x, y);
		return point.normalized; // Normalize the vector
	}

	static double NextDouble()
	{
		return _enemyRandom.NextDouble();
	}
}
