using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
	const int PointLevelScaling = 5;
	const int StartMinPoints = 20;
	const int StartMaxPoints = 20;
	const int TotalSpawnsPerLevel = 5;
	const int GroupOffsetDistanceMax = 3;

	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;

	[Header("Settings")]
	[SerializeField] int _minRadius = 30;
	[SerializeField] int _maxRadius = 40;
	[SerializeField] EnemyVariants _enemyVariants;
	[SerializeField] LevelSpecificSpawns _levelSpecificSpawns;

	[Header("Debug")]
	[SerializeField] bool _showGizmos;

	float _nextSpawnTime;
	float _timePerSpawn;
	int _spawnedWavesCurrentLevel;
	int _pointsRemainingFromLastSpawn;
	Dictionary<int, LevelSpawnConfiguration> _levelSpawnConfigurations;

	protected override void Awake()
	{
		base.Awake();

		InitializeLevelSpawnConfigurations();

		GameController.OnLevelChanged += OnLevelChanged;
	}

	void Update()
	{
		if (IsSpawning && Time.time >= _nextSpawnTime)
		{
			TrySpawn();
			_nextSpawnTime += _timePerSpawn;
		}
	}

	void InitializeLevelSpawnConfigurations()
	{
		_levelSpawnConfigurations = new Dictionary<int, LevelSpawnConfiguration>();
		foreach (var spawnOverride in _levelSpecificSpawns)
		{
			_levelSpawnConfigurations[spawnOverride.Level] = spawnOverride;
		}
	}

	void InitializeForLevel()
	{
		_timePerSpawn = GameController.Instance.TimePerLevel / TotalSpawnsPerLevel;
		_nextSpawnTime = Time.time;
		IsSpawning = true;
		_spawnedWavesCurrentLevel = 0;
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
		var pointsForThisSpawn = RandomManager.Enemy.Next(minPointsPerSpawn, maxPointsPerSpawn + 1);
		pointsForThisSpawn += _pointsRemainingFromLastSpawn;
		_pointsRemainingFromLastSpawn = 0;
		SpawnGroup(pointsForThisSpawn);
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

	void SpawnEnemy(GameObject prefab, Vector3 groupSpawnPosition)
	{
		// Slight random offset from the group's central spawn position
		var offset = new Vector3(RandomManager.Enemy.Next(-GroupOffsetDistanceMax, GroupOffsetDistanceMax), 0, RandomManager.Enemy.Next(-GroupOffsetDistanceMax, GroupOffsetDistanceMax)); // Offset range can be adjusted
		var spawnPosition = groupSpawnPosition + offset;
		var rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

		Instantiate(prefab, spawnPosition, rotation);
	}

	LevelSpawnConfiguration? GetLevelSpecificSpawn(int currentLevel)
	{
		if (_levelSpawnConfigurations.TryGetValue(currentLevel, out var spawnOverride))
		{
			return spawnOverride;
		}

		return null;
	}

	Vector3 GetRandomSpawnPosition()
	{
		var randomDirection = RandomManager.Enemy.InsideUnitCircleNormalized();
		return transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * RandomManager.Enemy.Next(_minRadius, _maxRadius));
	}

	Enemy ChooseEnemyToSpawn(int remainingPoints)
	{
		// Filter enemies that can be spawned within the remaining points
		var possibleEnemies = Array.FindAll(_enemyVariants.Enemies, e => e.Value <= remainingPoints);
		if (possibleEnemies.Length == 0)
		{
			return null;
		}

		return possibleEnemies[RandomManager.Enemy.Next(0, possibleEnemies.Length)];
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
		if (!_showGizmos)
		{
			return;
		}
		Gizmos.color = Color.red;

		// Draw circles for min and max radius, not spheres
		GizmosExtras.Draw2dCircle(transform.position, _minRadius);
		GizmosExtras.Draw2dCircle(transform.position, _maxRadius);
	}

	void OnLevelChanged()
	{
		InitializeForLevel();
	}
}
