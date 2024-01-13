using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] Transform _parentObject;
	[SerializeField] ObjectSpawnProfile[] _profiles;

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
	public void ExecuteSpawnProfiles()
	{
		if (!ValidateSettings())
		{
			return;
		}

		foreach (var profile in _profiles)
		{
			ExecuteSpawnProfile(profile);
		}
	}

	bool ValidateSettings()
	{
		var isValid = true;

		if (_profiles.Length == 0)
		{
			Debug.LogWarning("ObjectSpawner: No spawnable objects assigned.");
			isValid = false;
		}

		if (_parentObject == null)
		{
			Debug.LogWarning("ObjectSpawner: No parent object assigned. Objects will be parented to this script's GameObject.");
			isValid = false;
		}

		if (_parentObject == transform)
		{
			Debug.LogWarning("ObjectSpawner: Parent object is the same as this script's GameObject.");
			isValid = false;
		}

		// Add more validation checks as needed

		return isValid;
	}

	void ExecuteSpawnProfile(ObjectSpawnProfile profile)
	{

		for (var distance = profile.StartRadius; distance <= profile.EndRadius; distance++)
		{
			var normalizedDistance = Mathf.InverseLerp(profile.StartRadius, profile.EndRadius, distance);
			var spawnChanceModifier = Mathf.Lerp(0.1f, 1f, normalizedDistance);

			for (var i = 0; i < profile.Intensity; i++)
			{
				var randomValue = Random.value;
				if (randomValue > spawnChanceModifier)
				{
					continue;
				}
				TrySpawnObject(distance, profile);
			}
		}
	}

	void TrySpawnObject(int distance, ObjectSpawnProfile profile)
	{

		var randomDirection = Random.insideUnitCircle.normalized;
		var spawnPosition = transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * distance);
		var noiseValue = Mathf.PerlinNoise(spawnPosition.x * profile.NoiseScale, spawnPosition.z * profile.NoiseScale);
		var normalizedDistance = Mathf.InverseLerp(profile.StartRadius, profile.EndRadius, distance);
		var spawnChanceModifier = Mathf.Lerp(0.1f, 1f, normalizedDistance) * noiseValue;

		if (Random.value > spawnChanceModifier)
		{
			return;
		}

		if (!IsPositionObstructed(spawnPosition, profile.MinSpacing))
		{
			var spawnableObject = ChooseRandomObject(profile.Collection.Objects);
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

				if (profile.CarveNavMesh)
				{
					var navMeshObstacle = spawnedObject.AddComponent<NavMeshObstacle>();
					navMeshObstacle.carving = true;
					navMeshObstacle.size = Vector3.one * (Mathf.Max(scale, 1f) + 1f);
				}

				_spawnedObjects.Add(spawnedObject);
			}
		}
	}

	bool IsPositionObstructed(Vector3 position, float minSpacing)
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

	SpawnableObject ChooseRandomObject(SpawnableObject[] objects)
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

		foreach (var profile in _profiles)
		{
			DrawGizmosSettings(profile);
		}
	}

	void DrawGizmosSettings(ObjectSpawnProfile profile)
	{
		Gizmos.color = Color.yellow;
		GizmosExtras.Draw2dCircle(transform.position, profile.StartRadius);

		Gizmos.color = Color.blue;
		GizmosExtras.Draw2dCircle(transform.position, profile.EndRadius);

		Gizmos.color = Color.cyan;
		foreach (var obj in _spawnedObjects)
		{
			Gizmos.DrawWireCube(obj.transform.position, Vector3.one * profile.MinSpacing);
		}
	}
}
