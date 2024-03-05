using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectPlacer : MonoBehaviour
{
	const float OuterRadius = 120f; // Within this radius, all object types are allowed
	const float InnerRadius = 50f; // Within this radius, only details are allowed to not interfere with navigation

	[Header("Settings")]
	[SerializeField] Biome _biome;
	[SerializeField] Transform _parentObject;
	[SerializeField] bool _spawnOnStart;

	[Header("Types")]
	[SerializeField] bool _generateMountains;
	[SerializeField] bool _generateRoadsAndRivers;
	[SerializeField] bool _generateForests;
	[SerializeField] bool _generateDetails;
	[SerializeField] bool _generateIslands;

	[Header("Validation")]
	[SerializeField] float _heightCheckDistance = 100f;
	[SerializeField] LayerMask _obstacleLayerMask;
	[SerializeField] LayerMask _groundLayerMask;

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

	[Button]
	void GenerateMountains()
	{
		if (!_generateMountains)
		{
			return;
		}

		var mountainGenerator = new MountainGenerator(_biome, _parentObject, transform.position, OuterRadius, InnerRadius);
		mountainGenerator.Generate();
	}


	[Button]
	void GenerateRoadsAndRivers()
	{
		if (!_generateRoadsAndRivers)
		{
			return;
		}
		// TBD
	}

	[Button]
	void GenerateForests()
	{
		if (!_generateForests)
		{
			return;
		}

		var forestGenerator = new ForestGenerator(_biome, _parentObject, transform.position, OuterRadius, InnerRadius);
		forestGenerator.Generate();
	}

	[Button]
	void GenerateDetails()
	{
		if (!_generateDetails)
		{
			return;
		}
		// Small objects like rocks, bushes, and other details
		// These should be placed as single objects and with much lower intensity than the other types
	}

	[Button]
	void GenerateIslands()
	{
		if (!_generateIslands)
		{
			return;
		}
		// TBD
	}

	void OnEnable()
	{
		_biome.OnChanged += TryGenerate;
	}

	void OnDisable()
	{
		_biome.OnChanged -= TryGenerate;
	}
}
