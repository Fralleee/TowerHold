It didn't work. I don't think we're finding any edge vertices.
Can we instead save the edge vertices when we are generating the meshes and store them for this occasion?

Let me paste you my full code and you can apply those changes:

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FloatingIslandGenerator : MonoBehaviour
{
	public float Resolution = 256;
	public float Radius = 128f;
	public float MinHeight = 1f;
	public float ExitBevel = 1f;

	public float TopNoiseScale = 0.1f;
	public float TopNoiseHeight = 1f;

	public float BottomNoiseScalePeaks = 0.05f;
	public float BottomNoiseScaleDetails = 0.3f;
	public float BottomNoiseHeight = 128f;


	void Start()
	{
		GenerateIsland();
	}

	[Button]
	void GenerateIsland()
	{
		Mesh topMesh = GenerateTopPlane();
		Mesh bottomMesh = GenerateBottomPlane();

		List<Vector3> combinedVertices = new List<Vector3>();
		List<int> combinedTriangles = new List<int>();

		// Add top mesh vertices and triangles
		combinedVertices.AddRange(topMesh.vertices);
		combinedTriangles.AddRange(topMesh.triangles);

		int bottomMeshVertexOffset = combinedVertices.Count;

		// Add bottom mesh vertices and triangles
		combinedVertices.AddRange(bottomMesh.vertices);
		foreach (var tri in bottomMesh.triangles)
		{
			combinedTriangles.Add(tri + bottomMeshVertexOffset);
		}

		// Connect edges
		ConnectMeshEdges(combinedVertices, combinedTriangles, topMesh, bottomMesh, bottomMeshVertexOffset);

		Mesh combinedMesh = new Mesh();
		combinedMesh.vertices = combinedVertices.ToArray();
		combinedMesh.triangles = combinedTriangles.ToArray();
		combinedMesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = combinedMesh;
	}

	void ConnectMeshEdges(List<Vector3> combinedVertices, List<int> combinedTriangles, Mesh topMesh, Mesh bottomMesh, int bottomMeshVertexOffset)
	{
		// Find edge vertices based on their position
		List<int> topEdgeVertices = FindEdgeVertices(topMesh);
		List<int> bottomEdgeVertices = FindEdgeVertices(bottomMesh, bottomMeshVertexOffset);

		// Connect the edge vertices
		for (int i = 0; i < topEdgeVertices.Count; i++)
		{
			int nextIndex = (i + 1) % topEdgeVertices.Count;

			int topVertexIndex1 = topEdgeVertices[i];
			int topVertexIndex2 = topEdgeVertices[nextIndex];
			int bottomVertexIndex1 = bottomEdgeVertices[i];
			int bottomVertexIndex2 = bottomEdgeVertices[nextIndex];

			// Create triangles to connect the edges
			combinedTriangles.Add(topVertexIndex1);
			combinedTriangles.Add(bottomVertexIndex1);
			combinedTriangles.Add(topVertexIndex2);

			combinedTriangles.Add(topVertexIndex2);
			combinedTriangles.Add(bottomVertexIndex1);
			combinedTriangles.Add(bottomVertexIndex2);
		}
	}

	Mesh GenerateTopPlane()
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
					var height = Mathf.PerlinNoise(x * TopNoiseScale, y * TopNoiseScale) * TopNoiseHeight;
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

		return mesh;
	}

	Mesh GenerateBottomPlane()
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
					var distanceFromCenter = Mathf.Sqrt(xPos * xPos + yPos * yPos);
					var largeNoise = Mathf.PerlinNoise(xPos * BottomNoiseScalePeaks, yPos * BottomNoiseScalePeaks) * BottomNoiseHeight;
					var smallNoise = Mathf.PerlinNoise(xPos * BottomNoiseScaleDetails, yPos * BottomNoiseScaleDetails) * (BottomNoiseHeight / 3);
					var height = largeNoise + smallNoise;
					var normalizedDistance = 1 - (distanceFromCenter / Radius);
					height *= normalizedDistance;
					height += MinHeight;

					// Apply exit bevel
					if (distanceFromCenter > Radius - ExitBevel)
					{
						var bevelFactor = (Radius - distanceFromCenter) / ExitBevel;
						height *= bevelFactor;
					}

					vertices[i] = new Vector3(xPos, -height, yPos); // Invert height for the bottom plane
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
					triangles[tris + 0] = vert + 0;
					triangles[tris + 1] = vert + 1;
					triangles[tris + 2] = vert + (int)Resolution + 1;
					triangles[tris + 3] = vert + (int)Resolution + 1;
					triangles[tris + 4] = vert + (int)Resolution;
					triangles[tris + 5] = vert + 0;

					tris += 6;
				}
				vert++;
			}
			vert++;

		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		return mesh;
	}


	bool IsInsideCircle(Vector3[] vertices, int index, int res, float rad)
	{
		return vertices[index].x * vertices[index].x + vertices[index].z * vertices[index].z <= rad * rad &&
			   vertices[index + 1].x * vertices[index + 1].x + vertices[index + 1].z * vertices[index + 1].z <= rad * rad &&
			   vertices[index + res].x * vertices[index + res].x + vertices[index + res].z * vertices[index + res].z <= rad * rad &&
			   vertices[index + res + 1].x * vertices[index + res + 1].x + vertices[index + res + 1].z * vertices[index + res + 1].z <= rad * rad;
	}

	List<int> FindEdgeVertices(Mesh mesh, int offset = 0)
	{
		List<int> edgeVertices = new List<int>();

		for (int i = 0; i < mesh.vertexCount; i++)
		{
			Vector3 vertex = mesh.vertices[i];

			// Assuming vertices on the boundary have y-values at either the max or min
			if (Mathf.Abs(vertex.y - MinHeight) < 0.01f || Mathf.Abs(vertex.y - (MinHeight + TopNoiseHeight)) < 0.01f)
			{
				edgeVertices.Add(i + offset);
			}
		}

		return edgeVertices;
	}
}
