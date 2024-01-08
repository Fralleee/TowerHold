using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
	const int PointLevelScaling = 5;
	const int StartMinPoints = 20;
	const int StartMaxPoints = 20;
	const int TotalSpawnsPerLevel = 5;

	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;

	public int MinRadius = 30;
	public int MaxRadius = 40;
	public EnemyVariants EnemyVariants;
	public LevelSpecificSpawns LevelSpecificSpawns;

	[Header("Debug")]
	public bool ShowGizmos;


	float _nextSpawnTime;
	float _timePerSpawn;
	int _spawnedWavesCurrentLevel;
	int _pointsRemainingFromLastSpawn;
	readonly int _groupOffsetDistanceMax = 3;
	Dictionary<int, LevelSpawnConfiguration> _levelSpawnConfigurations;

	protected override void Awake()
	{
		base.Awake();

		_levelSpawnConfigurations = new Dictionary<int, LevelSpawnConfiguration>();
		foreach (var spawnOverride in LevelSpecificSpawns)
		{
			_levelSpawnConfigurations[spawnOverride.Level] = spawnOverride;
		}

		GameController.OnLevelChanged += OnLevelChanged;
	}


	void InitializeForLevel()
	{
		_timePerSpawn = GameController.Instance.TimePerLevel / TotalSpawnsPerLevel;
		CalculateNextSpawnTime(true);
		IsSpawning = true;
		_spawnedWavesCurrentLevel = 0;
	}

	void Update()
	{
		if (IsSpawning && Time.time >= _nextSpawnTime)
		{
			TrySpawn();
			CalculateNextSpawnTime();
		}
	}

	void CalculateNextSpawnTime(bool levelStart = false)
	{
		_nextSpawnTime = levelStart ? Time.time : _nextSpawnTime + _timePerSpawn;
	}

	void TrySpawn()
	{
		if (_spawnedWavesCurrentLevel == TotalSpawnsPerLevel)
		{
			return;
		}

		_spawnedWavesCurrentLevel++;
		var levelSpecificSpawn = GetLevelSpecificSpawn(GameController.Instance.CurrentLevel);
		if (levelSpecificSpawn.HasValue)
		{
			var spawnPosition = GetRandomSpawnPosition();
			foreach (var enemy in levelSpecificSpawn.Value.Enemies)
			{
				SpawnEnemy(enemy.gameObject, spawnPosition);
				if (!levelSpecificSpawn.Value.SpawnAsGroup)
				{
					spawnPosition = GetRandomSpawnPosition(); // Update position for next spawn
				}
			}

			if (levelSpecificSpawn.Value.SpawnOnce)
			{
				IsSpawning = false; // Stop spawning, this gets reset when the level changes
			}

			if (!levelSpecificSpawn.Value.ContinueDefaultSpawns)
			{
				return;
			}
		}

		var (minPointsPerSpawn, maxPointsPerSpawn) = CalculatePointsForLevel(GameController.Instance.CurrentLevel);
		var pointsForThisSpawn = RandomManager.Enemy(minPointsPerSpawn, maxPointsPerSpawn + 1);
		pointsForThisSpawn += _pointsRemainingFromLastSpawn;
		_pointsRemainingFromLastSpawn = 0;
		SpawnGroup(pointsForThisSpawn);
	}

	LevelSpawnConfiguration? GetLevelSpecificSpawn(int currentLevel)
	{
		if (_levelSpawnConfigurations.TryGetValue(currentLevel, out var spawnOverride))
		{
			return spawnOverride;
		}

		return null;
	}

	void SpawnGroup(int points)
	{
		if (points <= 0)
		{
			return;
		}

		// Determine a central spawn location for the group
		var groupSpawnPosition = GetRandomSpawnPosition();
		while (points > 0)
		{
			var enemyToSpawn = ChooseEnemyToSpawn(points);
			if (enemyToSpawn != null)
			{
				SpawnEnemy(enemyToSpawn.gameObject, groupSpawnPosition);
				points -= enemyToSpawn.Value;
			}
			else
			{
				_pointsRemainingFromLastSpawn = points;
				break; // No suitable enemy found within points
			}
		}
	}

	Vector3 GetRandomSpawnPosition()
	{
		var randomDirection = RandomManager.InsideUnitCircleNormalized();
		return transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * RandomManager.Enemy(MinRadius, MaxRadius));
	}

	Enemy ChooseEnemyToSpawn(int remainingPoints)
	{
		// Filter enemies that can be spawned within the remaining points
		var possibleEnemies = Array.FindAll(EnemyVariants.Enemies, e => e.Value <= remainingPoints);
		if (possibleEnemies.Length == 0)
		{
			return null;
		}

		return possibleEnemies[RandomManager.Enemy(0, possibleEnemies.Length)];
	}

	void SpawnEnemy(GameObject prefab, Vector3 groupSpawnPosition)
	{
		// Slight random offset from the group's central spawn position
		var offset = new Vector3(RandomManager.Enemy(-_groupOffsetDistanceMax, _groupOffsetDistanceMax), 0, RandomManager.Enemy(-_groupOffsetDistanceMax, _groupOffsetDistanceMax)); // Offset range can be adjusted
		var spawnPosition = groupSpawnPosition + offset;
		var rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

		Instantiate(prefab, spawnPosition, rotation);
	}


	(int minPoints, int maxPoints) CalculatePointsForLevel(int level)
	{
		var minPoints = StartMinPoints + (PointLevelScaling * (level - 1));
		var maxPoints = StartMaxPoints + (PointLevelScaling * (level - 1));
		return (minPoints, maxPoints);
	}


	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameController.OnLevelChanged -= OnLevelChanged;
	}

	void OnDrawGizmos()
	{
		if (!ShowGizmos)
		{
			return;
		}
		Gizmos.color = Color.red;

		// Draw circles for min and max radius, not spheres
		GizmosExtras.Draw2dCircle(transform.position, MinRadius);
		GizmosExtras.Draw2dCircle(transform.position, MaxRadius);
	}

	void OnLevelChanged()
	{
		InitializeForLevel();
	}
}
