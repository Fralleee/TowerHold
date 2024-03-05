using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "VAKT/Map/Biome")]
public class Biome : ScriptableObject
{
	public Action OnChanged = delegate { };

	[Header("Settings")]
	// All the prefabs will be using this material
	// Apply this to every prefab in the biome
	public Material Material;

	[Header("Mountains")]
	public GameObject[] MountainPrefabs;
	public float MountainFrequency = 0.5f;
	public float MountainNoiseScale = 0.01f;
	public Vector2 MountainScaleRange = new Vector2(0.6f, 1.2f);


	[Header("Forests")]
	public GameObject[] TreePrefabs;
	public Vector2 TreeScaleRange = new Vector2(0.8f, 1.2f);
	public Vector2 ForestLengthRange = new Vector2(32, 96);
	public Vector2 SeedRange = new Vector2(16, 24);
	public float DistanceBetweenTrees = 2f;
	public float TreeNoiseScale = 0.5f;
	public float TreeNoiseThreshold = 0.5f;

	[Header("Details")]
	public GameObject[] DetailsPrefabs;
	public float DetailsFrequency;


	// public GameObject[] RoadsAndRiversPrefabs;
	// public GameObject[] IslandPrefabs;

	// [Header("Environmental Conditions")]
	// public Color AmbientLightColor;
	// public float FogDensity;
	// Add more settings for weather, wind, etc.

	void OnValidate()
	{
		OnChanged();
	}
}
