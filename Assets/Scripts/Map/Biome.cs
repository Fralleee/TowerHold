using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "VAKT/Map/Biome")]
public class Biome : ScriptableObject
{
	public Action OnChanged = delegate { };

	[Header("Settings")]
	// All the prefabs will be using this material
	// Apply this to every prefab in the biome
	public Material Material;

	[ToggleGroup("Mountains", CollapseOthersOnExpand = false)] public bool Mountains;
	[ToggleGroup("Mountains")] public GameObject[] MountainPrefabs;
	[ToggleGroup("Mountains")] public float MountainFrequency = 0.5f;
	[ToggleGroup("Mountains")] public float MountainNoiseScale = 0.01f;
	[ToggleGroup("Mountains")] public Vector2 MountainScaleRange = new Vector2(0.6f, 1.2f);

	[ToggleGroup("Forests", CollapseOthersOnExpand = false)] public bool Forests;
	[ToggleGroup("Forests")] public GameObject[] TreePrefabs;
	[ToggleGroup("Forests")] public Vector2 TreeScaleRange = new Vector2(0.8f, 1.2f);
	[ToggleGroup("Forests")] public Vector2 ForestLengthRange = new Vector2(32, 96);
	[ToggleGroup("Forests")] public Vector2 SeedRange = new Vector2(16, 24);
	[ToggleGroup("Forests")] public float DistanceBetweenTrees = 2f;
	[ToggleGroup("Forests")] public float TreeNoiseScale = 0.5f;
	[ToggleGroup("Forests")] public float TreeNoiseThreshold = 0.5f;

	[ToggleGroup("Details", CollapseOthersOnExpand = false)] public bool Details;
	[ToggleGroup("Details")] public GameObject[] DetailsPrefabs;
	[ToggleGroup("Details")] public float DetailsInnerFrequency = 0.15f;
	[ToggleGroup("Details")] public float DetailsOuterFrequency = 0.2f;
	[ToggleGroup("Details")] public float DetailsNoiseScale = 0.5f;
	[ToggleGroup("Details")] public float DetailsNoiseThreshold = 0.5f;

	[ToggleGroup("Islands", CollapseOthersOnExpand = false)] public bool Islands;
	[ToggleGroup("Islands")] public GameObject[] IslandsPrefabs;
	[ToggleGroup("Islands")] public Vector2 IslandScaleRange = new Vector2(0.8f, 1.2f); // Scale range
	[ToggleGroup("Islands")] public float IslandsFrequency = 0.15f; // How frequently we spawn islands
	[ToggleGroup("Islands")] public float IslandDistanceFromOuterRadius = 20f; // How far outside the outer radius we allow island spawning

	// public GameObject[] RoadsAndRiversPrefabs;

	// [Header("Environmental Conditions")]
	// public Color AmbientLightColor;
	// public float FogDensity;

	// Add more settings for weather, wind, etc.
	// Instantiate effects based on these settings

	[Button(buttonSize: (int)ButtonSizes.Large)]
	public void Generate()
	{
		OnChanged();
	}

	void OnValidate()
	{
		OnChanged();
	}
}
