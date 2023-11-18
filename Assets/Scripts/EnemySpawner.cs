using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
	[HideInInspector] public bool IsSpawning;
	[HideInInspector] public Tower Target;
	float _lastSpawnTime = 0f;

	void Update()
	{
		if (IsSpawning && Time.time - _lastSpawnTime > GameController.Instance.EnemySpawnerSettings.SpawnRate)
		{
			Spawn();
			_lastSpawnTime = Time.time;
		}
	}

	void Spawn()
	{
		var randomDirection = Random.insideUnitCircle.normalized;
		var spawnPosition = transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(GameController.Instance.EnemySpawnerSettings.MinRadius, GameController.Instance.EnemySpawnerSettings.MaxRadius));
		var rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

		Instantiate(GameController.Instance.EnemySpawnerSettings.Prefab, spawnPosition, rotation);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		// Draw circles for min and max radius, not spheres
		Draw2dCircle(transform.position, GameController.Instance.EnemySpawnerSettings.MinRadius);
		Draw2dCircle(transform.position, GameController.Instance.EnemySpawnerSettings.MaxRadius);

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
