using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectPlacer : MonoBehaviour
{
	const float OuterRadius = 120f; // Within this radius, all object types are allowed
	const float InnerRadius = 50f; // Within this radius, only details are allowed to not interfere with navigation

	[Header("Settings")]
	[SerializeField] Biome _biome;
	[SerializeField] bool _spawnOnStart;

	Transform _parentObject;
	RandomGenerator _randomGenerator;

	void Start()
	{
		_randomGenerator = new RandomGenerator(GameController.GameSettings.StartSeed);
		if (_spawnOnStart && Application.isPlaying)
		{
			TryGenerate();
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
	}

	[Button]
	public void TryGenerate()
	{
		var startSeed = GameController.GameSettings != null ? GameController.GameSettings.StartSeed : 0;
		_randomGenerator ??= new RandomGenerator(startSeed);

		ClearObjects();
		if (!ValidateSettings())
		{
			return;
		}

		Generate();
	}


	bool ValidateSettings()
	{
		return true;
	}

	void Generate()
	{
		GenerateMountains();
		GenerateRoadsAndRivers();
		GenerateForests();
		GenerateDetails();
		GenerateIslands();
	}

	void GenerateMountains()
	{
		if (!_biome.Mountains)
		{
			return;
		}

		var mountainGenerator = new MountainGenerator(_biome, _parentObject, transform.position, OuterRadius, InnerRadius);
		mountainGenerator.Generate();
	}


	void GenerateRoadsAndRivers()
	{
		// TBD
	}

	void GenerateForests()
	{
		if (!_biome.Forests)
		{
			return;
		}

		var forestGenerator = new ForestGenerator(_biome, _parentObject, transform.position, OuterRadius, InnerRadius);
		forestGenerator.Generate();
	}

	void GenerateDetails()
	{
		if (!_biome.Details)
		{
			return;
		}

		var detailsGenerator = new DetailsGenerator(_biome, _parentObject, transform.position, OuterRadius, InnerRadius);
		detailsGenerator.Generate();
	}

	void GenerateIslands()
	{
		if (!_biome.Islands)
		{
			return;
		}

		var islandGenerator = new IslandGenerator(_biome, _parentObject, transform.position, OuterRadius);
		islandGenerator.Generate();
	}

	void OnEnable()
	{
		_biome.OnChanged += TryGenerate;

		_parentObject = transform.Find("Objects");
		if (!_parentObject)
		{
			var instance = Instantiate(new GameObject("Objects"), transform);
			instance.name = "Objects";
			_parentObject = instance.transform;
		}
	}

	void OnDisable()
	{
		_biome.OnChanged -= TryGenerate;
	}
}
