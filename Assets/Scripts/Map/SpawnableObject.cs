using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
	public GameObject Prefab;
	public float SpawnChance = 0.5f;
	public float MinScale = 0.85f;
	public float MaxScale = 1.15f;
	public Vector3 RotationAxis = Vector3.up;


	public static SpawnableObject ChooseRandomObject(SpawnableObject[] objects, RandomGenerator randomGenerator)
	{
		var totalSpawnChance = 0f;
		foreach (var spawnableObject in objects)
		{
			totalSpawnChance += spawnableObject.SpawnChance;
		}

		var randomValue = randomGenerator.NextFloat(0f, totalSpawnChance);
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
}
