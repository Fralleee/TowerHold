using Sirenix.OdinInspector;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
	public Material TerrainMaterial;
	public bool AutoUpdate;

	[SerializeField] MeshSettings _meshSettings;
	[SerializeField] HeightMapSettings _heightMapSettings;

	MeshFilter _meshFilter;

	void Awake()
	{
		_meshFilter = GetComponentInChildren<MeshFilter>();
	}

	[Button]
	public void DrawMapInEditor()
	{
		_meshFilter = GetComponentInChildren<MeshFilter>();

		var falloffMap = new HeightMap(HeightMapGenerator.GenerateFalloffMap(_meshSettings.NumVertsPerLine), 0, 1);
		var heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, Vector2.zero, falloffMap.Values);
		var meshData = MeshGenerator.GenerateTerrainMesh(heightMap.Values, _meshSettings, 0);

		_meshFilter.sharedMesh = meshData.CreateMesh();
	}
}
