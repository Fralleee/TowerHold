using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectPlacer : MonoBehaviour
{
	const float OuterRadius = 120f; // Within this radius, all object types are allowed
	const float InnerRadius = 50f; // Within this radius, only details are allowed to not interfere with navigation

	[Header("Settings")]
	[SerializeField] Biome _biome;
	[SerializeField] Transform _objectsContainer;
	[SerializeField] bool _spawnOnStart;

	RandomGenerator _randomGenerator;

	public static LayerMask GroundLayerMask => LayerMask.GetMask("Ground");
	public static int ObstacleLayer => LayerMask.NameToLayer("Obstacle");
	public static LayerMask ObstacleLayerMask => LayerMask.GetMask("Obstacle");

	void Start()
	{
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
		Debug.Log("Generating map objects");
		var startSeed = GameController.GameSettings != null ? GameController.GameSettings.StartSeed : 0;
		_randomGenerator = new RandomGenerator(startSeed);

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
		GenerateRoads();
		GenerateRivers();
		GenerateMountains();
		GenerateForests();
		GenerateDetails();
		GenerateIslands();
		GenerateEffects();
	}

	void GenerateRoads()
	{
		if (!_biome.Roads)
		{
			return;
		}

		var roadsGenerator = new RoadsGenerator(_biome, gameObject, _objectsContainer, transform.position);
		roadsGenerator.Generate();
		Physics.SyncTransforms();
	}

	void GenerateRivers()
	{
		if (!_biome.Rivers)
		{
			return;
		}

		var riversGenerator = new RiversGenerator(_biome, _randomGenerator, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		riversGenerator.Generate();
		Physics.SyncTransforms();
	}

	void GenerateMountains()
	{
		if (!_biome.Mountains)
		{
			return;
		}

		var mountainGenerator = new MountainGenerator(_biome, _randomGenerator, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		mountainGenerator.Generate();
		Physics.SyncTransforms();
	}

	void GenerateForests()
	{
		if (!_biome.Forests)
		{
			return;
		}

		var forestGenerator = new ForestGenerator(_biome, _randomGenerator, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		forestGenerator.Generate();
		Physics.SyncTransforms();
	}

	void GenerateDetails()
	{
		if (!_biome.Details)
		{
			return;
		}

		var detailsGenerator = new DetailsGenerator(_biome, _randomGenerator, _objectsContainer, transform.position, OuterRadius, InnerRadius);
		detailsGenerator.Generate();
		Physics.SyncTransforms();
	}

	void GenerateIslands()
	{
		if (!_biome.Islands)
		{
			return;
		}

		var islandGenerator = new IslandGenerator(_biome, _randomGenerator, _objectsContainer, transform.position, OuterRadius);
		islandGenerator.Generate();
		Physics.SyncTransforms();
	}

	void GenerateEffects()
	{
		if (!_biome.Effects)
		{
			return;
		}

		foreach (var effect in _biome.EffectsPrefabs)
		{
			_ = Instantiate(effect, _objectsContainer);
		}
	}

	public void SetBiome(Biome biome)
	{
		if (_biome != biome)
		{
			_biome.OnChanged -= TryGenerate;
		}

		_biome = biome;
		_biome.OnChanged += TryGenerate;
		TryGenerate();
	}

	void OnEnable()
	{
		_biome.OnChanged += TryGenerate;
	}

	void OnDisable()
	{
		_biome.OnChanged -= TryGenerate;
	}

	[SerializeField] bool _showGizmos;
	void OnDrawGizmos()
	{
		if (!_showGizmos)
		{
			return;
		}

		Gizmos.color = Color.red;
		GizmosExtras.Draw2dCircle(transform.position, InnerRadius);
		GizmosExtras.Draw2dCircle(transform.position, OuterRadius);
	}
}
