
using System;

public static class RandomManager
{
	public static void InitializeWithSeed(int seed)
	{
		World = new RandomGenerator(seed);
		Delay = new RandomGenerator(seed);
	}

	public static void InitializeWithSeedAndLevel(int seed)
	{
		Shop = new RandomGenerator(seed);
		Enemy = new RandomGenerator(seed);
	}

	public static void SetSeed(int startSeed, int currentLevel)
	{
		var hash = HashCode.Combine(startSeed, currentLevel);
		InitializeWithSeedAndLevel(hash);
		InitializeWithSeed(startSeed);
	}

	public static RandomGenerator World { get; private set; }
	public static RandomGenerator Shop { get; private set; }
	public static RandomGenerator Enemy { get; private set; }
	public static RandomGenerator Delay { get; private set; }
}
