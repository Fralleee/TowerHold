using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnProfile : ScriptableObject
{
	public ObjectSpawnerCollection[] Collections;
	public int StartRadius = 30;
	public int EndRadius = 40;
	public float MinSpacing = 1f;
	public int Intensity = 10;
	public bool CarveNavMesh;

	public SpawnableObject[] Objects
	{
		get
		{
			var combinedObjects = new List<SpawnableObject>();
			foreach (var collection in Collections)
			{
				if (collection != null && collection.Objects != null)
				{
					combinedObjects.AddRange(collection.Objects);
				}
			}

			return combinedObjects.ToArray();
		}
	}
}
