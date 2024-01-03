using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Singleton<EnemySpawner>
{
	public int MinRadius = 30;
	public int MaxRadius = 40;
	public float SpawnRate = 1f;
	public GameObject[] Prefabs;
	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;
	float _lastSpawnTime = 0f;

	[Header("Debug")]
	public bool ShowGizmos;

	void Update()
	{
		if (IsSpawning && Time.time - _lastSpawnTime > SpawnRate)
		{
			Spawn();
			_lastSpawnTime = Time.time;
		}
	}

	void Spawn()
	{
		var randomDirection = RandomManager.InsideUnitCircleNormalized();
		var spawnPosition = transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * RandomManager.Enemy(MinRadius, MaxRadius));
		var rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

		var prefab = Prefabs[RandomManager.Enemy(0, Prefabs.Length)];

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
