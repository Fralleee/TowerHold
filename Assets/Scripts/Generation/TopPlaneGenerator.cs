using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TopPlaneGenerator : MonoBehaviour
{
	public float Resolution = 256;
	public float Radius = 128f;
	public float NoiseScale = 0.3f;
	public float NoiseHeight = 5f;
	public float ExitBevel = 10f; // Width of the beveled edge

	void Start()
	{
		GenerateTopPlane();
	}

	[Button]
	void GenerateTopPlane()
	{
		var mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		var vertices = new Vector3[(int)Resolution * (int)Resolution];
		var triangles = new int[((int)Resolution - 1) * ((int)Resolution - 1) * 6];

		// Generate vertices
		var i = 0;
		for (var y = 0; y < Resolution; y++)
		{
			for (var x = 0; x < Resolution; x++, i++)
			{
				var xPos = (x / Resolution - 0.5f) * 2 * Radius;
				var yPos = (y / Resolution - 0.5f) * 2 * Radius;

				if (xPos * xPos + yPos * yPos <= Radius * Radius) // Inside the circle
				{
					var height = Mathf.PerlinNoise(x * NoiseScale, y * NoiseScale) * NoiseHeight;
					var distanceFromEdge = Radius - Mathf.Sqrt(xPos * xPos + yPos * yPos);

					// Apply exit bevel
					if (distanceFromEdge < ExitBevel)
					{
						height *= distanceFromEdge / ExitBevel;
					}

					vertices[i] = new Vector3(xPos, height, yPos);
				}
				else
				{
					vertices[i] = new Vector3(xPos, 0, yPos); // Outside the circle
				}
			}
		}

		// Generate triangles
		var vert = 0;
		var tris = 0;
		for (var y = 0; y < Resolution - 1; y++)
		{
			for (var x = 0; x < Resolution - 1; x++)
			{
				if (IsInsideCircle(vertices, vert, (int)Resolution, Radius))
				{
					triangles[tris] = vert;
					triangles[tris + 3] = triangles[tris + 2] = vert + 1;
					triangles[tris + 4] = triangles[tris + 1] = vert + (int)Resolution;
					triangles[tris + 5] = vert + (int)Resolution + 1;

					tris += 6;
				}
				vert++;
			}
			vert++;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	bool IsInsideCircle(Vector3[] vertices, int index, int res, float rad)
	{
		return vertices[index].x * vertices[index].x + vertices[index].z * vertices[index].z <= rad * rad &&
			   vertices[index + 1].x * vertices[index + 1].x + vertices[index + 1].z * vertices[index + 1].z <= rad * rad &&
			   vertices[index + res].x * vertices[index + res].x + vertices[index + res].z * vertices[index + res].z <= rad * rad &&
			   vertices[index + res + 1].x * vertices[index + res + 1].x + vertices[index + res + 1].z * vertices[index + res + 1].z <= rad * rad;
	}
}
