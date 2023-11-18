using System;
using UnityEngine;

[Serializable]
public class EnemySpawnerSettings
{
	public int MinRadius = 30;
	public int MaxRadius = 40;
	public float SpawnRate = 1f;
	public GameObject Prefab;
}
