using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class EnemyManager : Singleton<EnemyManager>
{
	public static SpatialPartitionManager SpatialPartitionManager => Instance._spatialPartitionManager;

	const int PointLevelScaling = 5;
	const int StartPoints = 200;
	const int TotalMinSpawnsPerLevel = 8;
	const int TotalMaxSpawnsPerLevel = 16;
	const int GroupOffsetDistanceMax = 3;
	const int ValueToDamageFactor = 5;

	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;

	[Header("Settings")]
	[SerializeField] int _minRadius = 30;
	[SerializeField] int _maxRadius = 40;
	[SerializeField] EnemyVariants _enemyVariants;
	[SerializeField] LevelSpecificSpawns _levelSpecificSpawns;

	Transform _enemies;
	float _nextSpawnTime;
	float _timePerSpawn;
	int _spawnedWavesCurrentLevel;
	int _spawnsPerLevel;
	int _totalPointsForCurrentLevel;

	int CalculatePointsForLevel(int level) => StartPoints + (PointLevelScaling * (level - 1));
	int DamageFromValue(Enemy enemy) => enemy.Value * ValueToDamageFactor;

	Dictionary<int, LevelSpawnConfiguration> _levelSpawnConfigurations;

	RandomGenerator _randomGenerator;
	SpatialPartitionManager _spatialPartitionManager;

	protected override void Awake()
	{
		base.Awake();

		InitializeLevelSpawnConfigurations();

		GameController.OnLevelChanged += OnLevelChanged;
	}

	void Start()
	{
		_randomGenerator = new RandomGenerator(GameController.GameSettings.StartSeed);
		_spatialPartitionManager = new SpatialPartitionManager();

		_enemies = new GameObject("Enemies").transform;
		_enemies.SetParent(transform);

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
		_totalPointsForCurrentLevel += CalculatePointsForLevel(GameController.Instance.CurrentLevel);
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
				SpawnEnemy(enemy, spawnPosition);
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
				// If we're not supposed to have default spawning we need to remove the points for the level
				_totalPointsForCurrentLevel -= CalculatePointsForLevel(GameController.Instance.CurrentLevel);
				return;
			}
		}

		var pointsForThisSpawn = Mathf.Min(_totalPointsForCurrentLevel / _spawnsPerLevel);
		_totalPointsForCurrentLevel -= pointsForThisSpawn;
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
				SpawnEnemy(enemyToSpawn, groupSpawnPosition);
				points -= enemyToSpawn.Value;
			}
			else
			{
				_totalPointsForCurrentLevel += points;
				break; // No suitable enemy found within points
			}
		}
	}

	void SpawnEnemy(Enemy enemy, Vector3 groupSpawnPosition)
	{
		if (Enemy.AliveEnemies >= GameController.GameSettings.MaxEnemiesAlive)
		{
			var damage = DamageFromValue(enemy);
			Debug.Log($"EnemyManager: Max enemies alive, dealing {damage} damage to target.");
			_ = Target.TakeDamage(damage);
		}

		// Slight random offset from the group's central spawn position
		var offset = new Vector3(_randomGenerator.Next(-GroupOffsetDistanceMax, GroupOffsetDistanceMax), 0, _randomGenerator.Next(-GroupOffsetDistanceMax, GroupOffsetDistanceMax)); // Offset range can be adjusted
		var spawnPosition = groupSpawnPosition + offset;
		var rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

		var instance = Instantiate(enemy.gameObject, spawnPosition, rotation, _enemies);
		instance.name = enemy.name;
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
		_ = StartCoroutine(DebugSpawnStaggered(_debugSpawnCount));
	}

	IEnumerator DebugSpawnStaggered(int count)
	{
		for (var i = 0; i < count; i++)
		{
			var spawnPosition = GetRandomSpawnPosition();
			SpawnEnemy(_debugEnemyType, spawnPosition);
			yield return new WaitForSeconds(0.1f);
		}
	}
	#endregion
}
