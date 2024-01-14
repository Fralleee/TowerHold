using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralFloatingIsland : MonoBehaviour
{
	public int Width = 50;
	public int Length = 50;
	public float HeightScale = 5f;
	public float BottomDepth = 20f;
	public float NoiseScale = 0.3f;

	void Start()
	{
		GenerateIsland();
	}

	[Button]
	void GenerateIsland()
	{
		var mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		var vertices = new Vector3[(Width + 1) * (Length + 1)];
		var triangles = new int[Width * Length * 6];

		for (int i = 0, z = 0; z <= Length; z++)
		{
			for (var x = 0; x <= Width; x++, i++)
			{
				var y = Mathf.PerlinNoise(x * NoiseScale, z * NoiseScale) * HeightScale;
				vertices[i] = new Vector3(x, y, z);

				// Invert bottom part
				if (y < 1f)
				{
					vertices[i].y -= BottomDepth;
				}
			}
		}

		var vert = 0;
		var tris = 0;
		for (var z = 0; z < Length; z++)
		{
			for (var x = 0; x < Width; x++)
			{
				triangles[tris + 0] = vert + 0;
				triangles[tris + 1] = vert + Width + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + Width + 1;
				triangles[tris + 5] = vert + Width + 2;

				vert++;
				tris += 6;
			}
			vert++;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}
}
