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
	[SerializeField] bool _spawnOnStart;

	[Header("Validation")]
	[SerializeField] float _heightCheckDistance = 100f;
	[SerializeField] LayerMask _obstacleLayerMask;
	[SerializeField] LayerMask _groundLayerMask;

	[Header("Debug")]
	[SerializeField] bool _showGizmos;

	RandomGenerator _randomGenerator;
	readonly List<GameObject> _spawnedObjects = new List<GameObject>();
	const int ROTATIONDEGREES = 180;

	void Start()
	{
		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);
		if (_spawnOnStart && Application.isPlaying)
		{
			ClearObjects();
			ExecuteSpawnProfiles();
		}
	}

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
		_randomGenerator ??= new RandomGenerator(GameController.Instance.StartSeed);

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
				var randomValue = _randomGenerator.NextFloat();
				if (randomValue > spawnChanceModifier)
				{
					continue;
				}
				if (profile is SingleSpawnProfile singleSpawnProfile)
				{
					TrySpawnObject(distance, singleSpawnProfile);
				}
				else if (profile is ClusterSpawnProfile clusterSpawnProfile)
				{
					TrySpawnCluster(distance, clusterSpawnProfile);
				}
			}
		}
	}

	void TrySpawnObject(int distance, SingleSpawnProfile profile)
	{
		var objects = profile.Objects;
		var randomValue = _randomGenerator.NextFloat();
		var randomDirection = _randomGenerator.InsideUnitCircle().normalized;
		var spawnPosition = transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * distance);

		var noiseValue = Mathf.PerlinNoise(spawnPosition.x * profile.NoiseScale, spawnPosition.z * profile.NoiseScale);
		var normalizedDistance = Mathf.InverseLerp(profile.StartRadius, profile.EndRadius, distance);
		var spawnChanceModifier = Mathf.Lerp(0.1f, 1f, normalizedDistance) * noiseValue;
		if (randomValue > spawnChanceModifier)
		{
			return;
		}

		var (isObstructed, newPosition) = IsPositionObstructed(spawnPosition, profile.MinSpacing);
		if (!isObstructed)
		{
			var spawnableObject = ChooseRandomObject(objects);
			if (spawnableObject != null)
			{
				var scale = _randomGenerator.NextFloat(spawnableObject.MinScale, spawnableObject.MaxScale);
				var rotation = new Vector3(
					_randomGenerator.NextFloat(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.x,
					_randomGenerator.NextFloat(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.y,
					_randomGenerator.NextFloat(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.z
				);

				var spawnedObject = Instantiate(spawnableObject.Prefab, newPosition ?? spawnPosition, Quaternion.Euler(rotation), _parentObject);
				spawnedObject.transform.localScale = new Vector3(scale, scale, scale);

				if (profile.CarveNavMesh)
				{
					var navMeshObstacle = spawnedObject.AddComponent<NavMeshObstacle>();
					navMeshObstacle.carving = true;
					navMeshObstacle.size = new Vector3(Mathf.Max(scale, 1f) + 1f, 10f, Mathf.Max(scale, 1f) + 1f);
				}

				_spawnedObjects.Add(spawnedObject);
			}
		}
	}

	void TrySpawnCluster(int distance, ClusterSpawnProfile profile)
	{
		var objects = profile.Objects;
		var clusterCenter = GetRandomSpawnPosition(distance);
		var clusterSize = _randomGenerator.Next(profile.MinClusterSize, profile.MaxClusterSize);
		for (var i = 0; i < clusterSize; i++)
		{
			var clusterRadius = clusterSize * profile.MinSpacing;
			var offset = _randomGenerator.InsideUnitSphere() * clusterRadius;
			offset.y = 0;  // Keep the offset in the horizontal plane
			var spawnPosition = clusterCenter + offset;

			var (isObstructed, newPosition) = IsPositionObstructed(spawnPosition, profile.MinSpacing);
			if (!isObstructed)
			{
				var spawnableObject = ChooseRandomObject(objects);
				if (spawnableObject != null)
				{
					var scale = _randomGenerator.NextFloat(spawnableObject.MinScale, spawnableObject.MaxScale);
					var rotation = new Vector3(
						_randomGenerator.NextFloat(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.x,
						_randomGenerator.NextFloat(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.y,
						_randomGenerator.NextFloat(-ROTATIONDEGREES, ROTATIONDEGREES) * spawnableObject.RotationAxis.z
					);

					var spawnedObject = Instantiate(spawnableObject.Prefab, newPosition ?? spawnPosition, Quaternion.Euler(rotation), _parentObject);
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

	}

	Vector3 GetRandomSpawnPosition(int distance)
	{
		var randomDirection = _randomGenerator.InsideUnitCircle().normalized;
		return transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * distance);
	}

	(bool isObstructed, Vector3? newPosition) IsPositionObstructed(Vector3 position, float minSpacing)
	{
		foreach (var obj in _spawnedObjects)
		{
			if (Vector3.Distance(obj.transform.position, position) < minSpacing)
			{
				return (true, null);
			}
		}

		var ray = new Ray(position + (Vector3.up * _heightCheckDistance), Vector3.down);
		if (Physics.Raycast(ray, out var hit, _heightCheckDistance + 10f, _obstacleLayerMask | _groundLayerMask))
		{
			if ((_groundLayerMask.value & (1 << hit.collider.gameObject.layer)) != 0)
			{
				return (false, hit.point);
			}
			return (true, null);
		}

		return (false, position);
	}

	SpawnableObject ChooseRandomObject(SpawnableObject[] objects)
	{
		var totalSpawnChance = 0f;
		foreach (var spawnableObject in objects)
		{
			totalSpawnChance += spawnableObject.SpawnChance;
		}

		var randomValue = _randomGenerator.NextFloat(0f, totalSpawnChance);
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
