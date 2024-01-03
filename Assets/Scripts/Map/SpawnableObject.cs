using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
	public GameObject Prefab;
	public float SpawnChance = 0.5f;
	public float MinScale = 0.85f;
	public float MaxScale = 1.15f;
	public Vector3 RotationAxis = Vector3.up;
}
