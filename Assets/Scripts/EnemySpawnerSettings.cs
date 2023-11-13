using System;
using UnityEngine;

[Serializable]
public class EnemySpawnerSettings
{
  public int minRadius = 30;
  public int maxRadius = 40;
  public float spawnRate = 1f;
  public GameObject prefab;
}
