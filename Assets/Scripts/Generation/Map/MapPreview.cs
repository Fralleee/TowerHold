using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
	public Material TerrainMaterial;

	[SerializeField] MeshSettings _meshSettings;
	[SerializeField] HeightMapSettings _heightMapSettings;

	MeshFilter _meshFilter;
	MeshCollider _meshCollider;
	NavMeshSurface _navMeshSurface;
	RandomGenerator _randomGenerator;

	void Start()
	{
		BuildMap();

		_navMeshSurface.BuildNavMesh();
	}

	[Button]
	public void ClearMap()
	{
		_meshFilter = GetComponentInChildren<MeshFilter>();
		_meshCollider = GetComponentInChildren<MeshCollider>();
		_navMeshSurface = GetComponentInChildren<NavMeshSurface>();

		_meshFilter.sharedMesh = null;
		_meshCollider.sharedMesh = null;
		_navMeshSurface.RemoveData();
	}

	[Button]
	public void BuildMap()
	{
		_meshFilter = GetComponentInChildren<MeshFilter>();
		_meshCollider = GetComponentInChildren<MeshCollider>();
		_navMeshSurface = GetComponentInChildren<NavMeshSurface>();
		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);

		var heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _randomGenerator, _heightMapSettings, Vector2.zero);
		var meshData = MeshGenerator.GenerateTerrainMesh(heightMap.Values, _meshSettings, 0);

		var mesh = meshData.CreateMesh();

		_meshFilter.sharedMesh = mesh;

		if (_meshCollider != null)
		{
			_meshCollider.sharedMesh = null; // Clear the current mesh (important for updating)
			_meshCollider.sharedMesh = mesh;
		}
		else
		{
			Debug.LogError("MeshCollider component not found on this GameObject.");
		}
	}
}
