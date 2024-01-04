using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
	public int MinRadius = 30;
	public int MaxRadius = 40;
	public float SpawnRate = 1f;
	public EnemyVariants EnemyVariants;
	public LevelSpecificSpawns LevelSpecificSpawns;
	public int MinPointsPerSpawn = 4;
	public int MaxPointsPerSpawn = 6;
	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;

	[Header("Debug")]
	public bool ShowGizmos;

	float _lastSpawnTime;
	int _pointsRemainingFromLastSpawn;
	readonly int _groupOffsetDistanceMax = 3;
	Dictionary<int, LevelSpawnConfiguration> _levelSpawnConfigurations;

	protected override void Awake()
	{
		base.Awake();

		_lastSpawnTime = -SpawnRate + 0.1f; // Spawn immediately on start
		_levelSpawnConfigurations = new Dictionary<int, LevelSpawnConfiguration>();
		foreach (var spawnOverride in LevelSpecificSpawns)
		{
			_levelSpawnConfigurations[spawnOverride.Level] = spawnOverride;
		}
	}

	void Update()
	{
		if (IsSpawning && Time.time - _lastSpawnTime > SpawnRate)
		{
			TrySpawn();
			_lastSpawnTime += SpawnRate;
		}
	}

	void TrySpawn()
	{
		var spawnOverride = GetSpawnOverride(GameController.Instance.CurrentLevel);
		if (spawnOverride.HasValue)
		{
			var spawnPosition = GetRandomSpawnPosition();
			foreach (var enemy in spawnOverride.Value.Enemies)
			{
				SpawnEnemy(enemy.gameObject, spawnPosition);
				if (!spawnOverride.Value.SpawnAsGroup)
				{
					spawnPosition = GetRandomSpawnPosition(); // Update position for next spawn
				}
			}
		}
		else
		{
			var pointsForThisSpawn = RandomManager.Enemy(MinPointsPerSpawn, MaxPointsPerSpawn + 1);
			pointsForThisSpawn += _pointsRemainingFromLastSpawn;
			_pointsRemainingFromLastSpawn = 0;
			SpawnGroup(pointsForThisSpawn);
		}
	}

	LevelSpawnConfiguration? GetSpawnOverride(int currentLevel)
	{
		Debug.Log($"Checking for spawn override for level {currentLevel}");
		if (_levelSpawnConfigurations.TryGetValue(currentLevel, out var spawnOverride))
		{
			Debug.Log($"Found spawn override for level {currentLevel}");
			return spawnOverride;
		}

		Debug.Log($"No spawn override for level {currentLevel}");
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


	void OnDrawGizmos()
	{
		if (!ShowGizmos)
		{
			return;
		}
		Gizmos.color = Color.red;

		// Draw circles for min and max radius, not spheres
		Draw2dCircle(transform.position, MinRadius);
		Draw2dCircle(transform.position, MaxRadius);

	}

	void Draw2dCircle(Vector3 center, float radius)
	{
		var prevPos = center + new Vector3(radius, 0, 0);
		for (var i = 0; i < 30; i++)
		{
			var angle = i / 30f * Mathf.PI * 2f;
			var newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
			Gizmos.DrawLine(prevPos, newPos);
			prevPos = newPos;
		}
	}
}
