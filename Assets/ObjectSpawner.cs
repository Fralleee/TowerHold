using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
	[Header("Spawn Settings")]
	public SpawnableObject[] SpawnableObjects;
	public Transform ParentObject;
	public int StartRadius = 30;
	public int EndRadius = 40;
	public LayerMask ObstructionLayer;
	public float MinSpacing = 1f;
	public int Intensity = 10;

	[Header("Debug")]
	public bool ShowGizmos;

	readonly List<GameObject> _spawnedObjects = new List<GameObject>();
	const int ROTATIONDEGRESS = 180;

	[Button]
	public void ClearObjects()
	{
		if (ParentObject == null)
		{
			Debug.LogWarning("No parent object assigned. Objects will be parented to this script's GameObject.");
			return;
		}

		if (ParentObject == transform)
		{
			Debug.LogWarning("Parent object is the same as this script's GameObject.");
			return;
		}

		// Iterate all children in parentobject
		for (var i = ParentObject.childCount - 1; i >= 0; i--)
		{
			var child = ParentObject.GetChild(i);
			DestroyImmediate(child.gameObject);
		}

		_spawnedObjects.Clear();
	}

	[Button]
	public void GenerateObjects()
	{
		if (SpawnableObjects.Length == 0)
		{
			Debug.LogWarning("No spawnable objects assigned.");
			return;
		}

		if (ParentObject == null)
		{
			Debug.LogWarning("No parent object assigned. Objects will be parented to this script's GameObject.");
			return;
		}

		if (ParentObject == transform)
		{
			Debug.LogWarning("Parent object is the same as this script's GameObject.");
			return;
		}

		for (var distance = StartRadius; distance <= EndRadius; distance++)
		{
			var normalizedDistance = Mathf.InverseLerp(StartRadius, EndRadius, distance);
			var spawnChanceModifier = Mathf.Lerp(0.1f, 1f, normalizedDistance);

			for (var i = 0; i < Intensity; i++)
			{
				var randomValue = Random.value;
				if (randomValue > spawnChanceModifier)
				{
					continue;
				}

				var randomDirection = Random.insideUnitCircle.normalized;
				var spawnPosition = transform.position + (new Vector3(randomDirection.x, 0, randomDirection.y) * distance);

				if (!IsObstructed(spawnPosition))
				{
					var spawnableObject = SelectRandomObject();
					if (spawnableObject != null)
					{
						var scale = Random.Range(spawnableObject.MinScale, spawnableObject.MaxScale);
						var rotation = new Vector3(0f, Random.Range(-ROTATIONDEGRESS, ROTATIONDEGRESS), 0f);

						var spawnedObject = Instantiate(spawnableObject.Prefab, spawnPosition, Quaternion.Euler(rotation), ParentObject);
						spawnedObject.transform.localScale = new Vector3(scale, scale, scale);

						_spawnedObjects.Add(spawnedObject);
					}
				}
			}
		}
	}

	bool IsObstructed(Vector3 position)
	{
		foreach (var obj in _spawnedObjects)
		{
			if (Vector3.Distance(obj.transform.position, position) < MinSpacing)
			{
				return true;
			}
		}
		return false;
	}

	SpawnableObject SelectRandomObject()
	{
		var totalSpawnChance = 0f;
		foreach (var spawnableObject in SpawnableObjects)
		{
			totalSpawnChance += spawnableObject.SpawnChance;
		}

		var randomValue = Random.Range(0f, totalSpawnChance);
		foreach (var spawnableObject in SpawnableObjects)
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
		if (!ShowGizmos)
		{
			return;
		}

		Gizmos.color = Color.yellow;
		Draw2dCircle(transform.position, StartRadius);

		Gizmos.color = Color.red;
		Draw2dCircle(transform.position, EndRadius);

		Gizmos.color = Color.cyan;
		foreach (var obj in _spawnedObjects)
		{
			Gizmos.DrawWireCube(obj.transform.position, Vector3.one * MinSpacing);
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
