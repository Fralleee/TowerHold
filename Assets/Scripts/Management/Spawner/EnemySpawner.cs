using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class EnemySpawner : Singleton<EnemySpawner>
{
	const int PointLevelScaling = 5;
	const int StartMinPoints = 20;
	const int StartMaxPoints = 20;
	const int TotalMinSpawnsPerLevel = 10;
	const int TotalMaxSpawnsPerLevel = 15;
	const int GroupOffsetDistanceMax = 3;

	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;

	[Header("Settings")]
	[SerializeField] int _minRadius = 30;
	[SerializeField] int _maxRadius = 40;
	[SerializeField] EnemyVariants _enemyVariants;
	[SerializeField] LevelSpecificSpawns _levelSpecificSpawns;

	float _nextSpawnTime;
	float _timePerSpawn;
	int _spawnedWavesCurrentLevel;
	int _pointsRemainingFromLastSpawn;
	int _spawnsPerLevel;
	Dictionary<int, LevelSpawnConfiguration> _levelSpawnConfigurations;

	RandomGenerator _randomGenerator;

	protected override void Awake()
	{
		base.Awake();

		InitializeLevelSpawnConfigurations();

		GameController.OnLevelChanged += OnLevelChanged;
	}

	void Start()
	{
		_randomGenerator = new RandomGenerator(GameController.GameSettings.StartSeed);

		_nextSpawnTime = Time.time + 0.1f;
	}

	void FixedUpdate()
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
		_spawnsPerLevel = _randomGenerator.Next(TotalMinSpawnsPerLevel, TotalMaxSpawnsPerLevel + 1);
		_timePerSpawn = GameController.GameSettings.TimePerLevel / _spawnsPerLevel;
		_nextSpawnTime = Time.time;
		IsSpawning = true;
		_spawnedWavesCurrentLevel = 0;
	}

	void TrySpawn()
	{
		if (_spawnedWavesCurrentLevel == _spawnsPerLevel)
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
		var pointsForThisSpawn = _randomGenerator.Next(minPointsPerSpawn, maxPointsPerSpawn + 1);
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
		var offset = new Vector3(_randomGenerator.Next(-GroupOffsetDistanceMax, GroupOffsetDistanceMax), 0, _randomGenerator.Next(-GroupOffsetDistanceMax, GroupOffsetDistanceMax)); // Offset range can be adjusted
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
		var randomDirection = _randomGenerator.InsideUnitCircle().normalized;
		return transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * _randomGenerator.Next(_minRadius, _maxRadius));
	}

	Enemy ChooseEnemyToSpawn(int remainingPoints)
	{
		// Filter enemies that can be spawned within the remaining points
		var possibleEnemies = Array.FindAll(_enemyVariants.Enemies, e => e.Value <= remainingPoints);
		if (possibleEnemies.Length == 0)
		{
			return null;
		}

		return possibleEnemies[_randomGenerator.Next(0, possibleEnemies.Length)];
	}

	(int minPoints, int maxPoints) CalculatePointsForLevel(int level)
	{
		var minPoints = StartMinPoints + (PointLevelScaling * (level - 1));
		var maxPoints = StartMaxPoints + (PointLevelScaling * (level - 1));
		return (minPoints, maxPoints);
	}


	IEnumerable<Enemy> GetEnemyTypes()
	{
		return _enemyVariants.Enemies;
	}

	void OnLevelChanged(int level)
	{
		InitializeForLevel();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameController.OnLevelChanged -= OnLevelChanged;
	}

	void OnValidate()
	{
		if (_enemyVariants.Enemies.Length > 0 && _debugEnemyType == null)
		{
			_debugEnemyType = _enemyVariants.Enemies[0];
		}
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

	#region Debug
	[Header("Debug")]
	[SerializeField] bool _showGizmos;
	[FoldoutGroup("Custom Spawn", expanded: false)]
	[ShowInInspector, LabelText("Number of Enemies"), Range(1, 100)]
	readonly int _debugSpawnCount = 1;

	[FoldoutGroup("Custom Spawn")]
	[ShowInInspector, LabelText("Enemy Type"), ValueDropdown(nameof(GetEnemyTypes))]
	Enemy _debugEnemyType;


	[FoldoutGroup("Custom Spawn")]
	[Button("Spawn Enemies")]
	void DebugSpawnEnemies()
	{
		StartCoroutine(DebugSpawnStaggered(_debugSpawnCount));
	}

	IEnumerator DebugSpawnStaggered(int count)
	{
		for (var i = 0; i < count; i++)
		{
			var spawnPosition = GetRandomSpawnPosition();
			SpawnEnemy(_debugEnemyType.gameObject, spawnPosition);
			yield return new WaitForSeconds(0.1f);
		}
	}
	#endregion
}
