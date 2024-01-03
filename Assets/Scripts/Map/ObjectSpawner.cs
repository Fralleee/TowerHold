using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
	[Header("Spawn Settings")]
	[SerializeField] Transform _parentObject;
	[SerializeField] ObjectSpawnerSettings[] _settings;

	[Header("Debug")]
	[SerializeField] bool _showGizmos;

	readonly List<GameObject> _spawnedObjects = new List<GameObject>();
	const int ROTATIONDEGREES = 180;

	[Button]
	public void ClearObjects()
	{
		if (_parentObject == null)
		{
			Debug.LogWarning("No parent object assigned. Objects will be parented to this script's GameObject.");
			return;
		}

		if (_parentObject == transform)
		{
			Debug.LogWarning("Parent object is the same as this script's GameObject.");
			return;
		}

		for (var i = _parentObject.childCount - 1; i >= 0; i--)
		{
			var child = _parentObject.GetChild(i);
			DestroyImmediate(child.gameObject);
		}

		_spawnedObjects.Clear();
	}

	[Button]
	public void GenerateObjects()
	{
		if (_settings.Length == 0)
		{
			Debug.LogWarning("No spawnable objects assigned.");
			return;
		}

		if (_parentObject == null)
		{
			Debug.LogWarning("No parent object assigned. Objects will be parented to this script's GameObject.");
			return;
		}

		if (_parentObject == transform)
		{
			Debug.LogWarning("Parent object is the same as this script's GameObject.");
			return;
		}

		foreach (var setting in _settings)
		{
			GenerateObjectsSettings(setting);
		}
	}

	void GenerateObjectsSettings(ObjectSpawnerSettings settings)
	{

		for (var distance = settings.StartRadius; distance <= settings.EndRadius; distance++)
		{
			var normalizedDistance = Mathf.InverseLerp(settings.StartRadius, settings.EndRadius, distance);
			var spawnChanceModifier = Mathf.Lerp(0.1f, 1f, normalizedDistance);

			for (var i = 0; i < settings.Intensity; i++)
			{
				var randomValue = Random.value;
				if (randomValue > spawnChanceModifier)
				{
					continue;
				}
				SpawnObject(distance, settings);
			}
		}
	}

	void SpawnObject(int distance, ObjectSpawnerSettings settings)
	{

		var randomDirection = Random.insideUnitCircle.normalized;
		var spawnPosition = transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * distance);

		if (!IsObstructed(spawnPosition, settings.MinSpacing))
		{
			var spawnableObject = SelectRandomObject(settings.Collection.Objects);
			if (spawnableObject != null)
			{
				var scale = Random.Range(spawnableObject.MinScale, spawnableObject.MaxScale);
				var rotation = new Vector3(
					Random.Range(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.x,
					Random.Range(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.y,
					Random.Range(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.z
				);

				var spawnedObject = Instantiate(spawnableObject.Prefab, spawnPosition, Quaternion.Euler(rotation), _parentObject);
				spawnedObject.transform.localScale = new Vector3(scale, scale, scale);

				if (settings.CarveNavMesh)
				{
					var navMeshObstacle = spawnedObject.AddComponent<NavMeshObstacle>();
					navMeshObstacle.carving = true;
					navMeshObstacle.size = Vector3.one * (Mathf.Max(scale, 1f) + 1f);
				}

				_spawnedObjects.Add(spawnedObject);
			}
		}
	}

	bool IsObstructed(Vector3 position, float minSpacing)
	{
		foreach (var obj in _spawnedObjects)
		{
			if (Vector3.Distance(obj.transform.position, position) < minSpacing)
			{
				return true;
			}
		}
		return false;
	}

	SpawnableObject SelectRandomObject(SpawnableObject[] objects)
	{
		var totalSpawnChance = 0f;
		foreach (var spawnableObject in objects)
		{
			totalSpawnChance += spawnableObject.SpawnChance;
		}

		var randomValue = Random.Range(0f, totalSpawnChance);
		foreach (var spawnableObject in objects)
		{
			if (randomValue <= spawnableObject.SpawnChance)
			{
				return spawnableObject;
			}
			else
			{
				randomValue -= spawnableObject.SpawnChance;
			}
		}

		return null;
	}

	void OnDrawGizmos()
	{
		if (!_showGizmos)
		{
			return;
		}

		foreach (var setting in _settings)
		{
			DrawGizmosSettings(setting);
		}
	}

	void DrawGizmosSettings(ObjectSpawnerSettings settings)
	{
		Gizmos.color = Color.yellow;
		Draw2dCircle(transform.position, settings.StartRadius);

		Gizmos.color = Color.blue;
		Draw2dCircle(transform.position, settings.EndRadius);

		Gizmos.color = Color.cyan;
		foreach (var obj in _spawnedObjects)
		{
			Gizmos.DrawWireCube(obj.transform.position, Vector3.one * settings.MinSpacing);
		}
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
