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

	Transform _objectsContainer;
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
		if (_objectsContainer == null)
		{
			Debug.LogWarning("No parent object assigned. Objects will be parented to this script's GameObject.");
			return;
		}

		if (_objectsContainer == transform)
		{
			Debug.LogWarning("Parent object is the same as this script's GameObject.");
			return;
		}

		for (var i = _objectsContainer.childCount - 1; i >= 0; i--)
		{
			var child = _objectsContainer.GetChild(i);
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
		GenerateRoadsAndRivers();
		Physics.SyncTransforms();
		GenerateMountains();
		Physics.SyncTransforms();
		GenerateForests();
		Physics.SyncTransforms();
		GenerateDetails();
		Physics.SyncTransforms();
		GenerateIslands();
		Physics.SyncTransforms();
	}

	void GenerateMountains()
	{
		if (!_biome.Mountains)
		{
			return;
		}

		var mountainGenerator = new MountainGenerator(_biome, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		mountainGenerator.Generate();
	}


	void GenerateRoadsAndRivers()
	{
		if (!_biome.Rivers)
		{
			return;
		}

		var roadsAndRiversGenerator = new RiversGenerator(_biome, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		roadsAndRiversGenerator.Generate();
	}

	void GenerateForests()
	{
		if (!_biome.Forests)
		{
			return;
		}

		var forestGenerator = new ForestGenerator(_biome, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		forestGenerator.Generate();
	}

	void GenerateDetails()
	{
		if (!_biome.Details)
		{
			return;
		}

		var detailsGenerator = new DetailsGenerator(_biome, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		detailsGenerator.Generate();
	}

	void GenerateIslands()
	{
		if (!_biome.Islands)
		{
			return;
		}

		var islandGenerator = new IslandGenerator(_biome, _objectsContainer, transform.position, OuterRadius);
		islandGenerator.Generate();
	}

	void OnEnable()
	{
		_biome.OnChanged += TryGenerate;

		_objectsContainer = transform.Find("Objects");
		if (!_objectsContainer)
		{
			var instance = Instantiate(new GameObject("Objects"), transform);
			instance.name = "Objects";
			_objectsContainer = instance.transform;
		}
	}

	void OnDisable()
	{
		_biome.OnChanged -= TryGenerate;
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		GizmosExtras.Draw2dCircle(transform.position, InnerRadius);
		GizmosExtras.Draw2dCircle(transform.position, OuterRadius);
	}
}
