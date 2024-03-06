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

	[ToggleGroup("Rivers", CollapseOthersOnExpand = false)] public bool Rivers;
	[ToggleGroup("Rivers")] public GameObject RiverPrefab;
	[ToggleGroup("Rivers"), MaxValue(10)] public int TotalRivers = 5;
	[ToggleGroup("Rivers")] public float MinDistanceBetweenOtherPoints = 8f;
	[ToggleGroup("Rivers"), MinMaxSlider(3, 25, true)] public Vector2Int PointsPerRiverRange = new Vector2Int(6, 12);
	[ToggleGroup("Rivers"), MinMaxSlider(10, 25, true)] public Vector2 DistanceBetweenPointsRange = new Vector2(10f, 25f);
	[ToggleGroup("Rivers"), MinMaxSlider(2, 10, true)] public Vector2 RiverWidthRange = new Vector2(4f, 8f);
	[ToggleGroup("Rivers")] public float RiversNoiseScale = 0.5f;

	[ToggleGroup("Mountains", CollapseOthersOnExpand = false)] public bool Mountains;
	[ToggleGroup("Mountains")] public GameObject[] MountainPrefabs;
	[ToggleGroup("Mountains")] public float MountainFrequency = 0.5f;
	[ToggleGroup("Mountains")] public float MountainNoiseScale = 0.01f;
	[ToggleGroup("Mountains"), MinMaxSlider(0.1f, 5f, true)] public Vector2 MountainScaleRange = new Vector2(0.6f, 1.2f);

	[ToggleGroup("Forests", CollapseOthersOnExpand = false)] public bool Forests;
	[ToggleGroup("Forests")] public GameObject[] TreePrefabs;
	[ToggleGroup("Forests"), MinMaxSlider(0.1f, 5f, true)] public Vector2 TreeScaleRange = new Vector2(0.8f, 1.2f);
	[ToggleGroup("Forests"), MinMaxSlider(3, 128, true)] public Vector2Int ForestLengthRange = new Vector2Int(32, 96);
	[ToggleGroup("Forests"), MinMaxSlider(1, 32, true)] public Vector2Int SeedRange = new Vector2Int(16, 24);
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
	[ToggleGroup("Islands"), MinMaxSlider(0.1f, 5f, true)] public Vector2 IslandScaleRange = new Vector2(0.8f, 1.2f);
	[ToggleGroup("Islands")] public float IslandsFrequency = 0.15f;
	[ToggleGroup("Islands")] public float IslandDistanceFromOuterRadius = 20f;


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
