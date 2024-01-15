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

	List<int> _topEdgeVerticesIndices;
	List<int> _bottomEdgeVerticesIndices;
	float EdgeVertexThreshold => (Radius - ExitBevel) * (Radius - ExitBevel);

	void Start()
	{
		GenerateIsland();
	}

	[Button]
	void GenerateIsland()
	{
		_topEdgeVerticesIndices = new List<int>();
		_bottomEdgeVerticesIndices = new List<int>();

		var topMesh = GenerateTopPlane();
		var bottomMesh = GenerateBottomPlane();

		var combinedVertices = new List<Vector3>();
		var combinedTriangles = new List<int>();

		// Add top mesh vertices and triangles
		combinedVertices.AddRange(topMesh.vertices);
		combinedTriangles.AddRange(topMesh.triangles);

		var bottomMeshVertexOffset = combinedVertices.Count;

		// Add bottom mesh vertices and triangles
		combinedVertices.AddRange(bottomMesh.vertices);
		foreach (var tri in bottomMesh.triangles)
		{
			combinedTriangles.Add(tri + bottomMeshVertexOffset);
		}

		// Connect edges using stored edge vertices
		ConnectMeshEdges(combinedVertices, combinedTriangles);

		var combinedMesh = new Mesh
		{
			vertices = combinedVertices.ToArray(),
			triangles = combinedTriangles.ToArray()
		};
		combinedMesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = combinedMesh;
	}

	void ConnectMeshEdges(List<Vector3> combinedVertices, List<int> combinedTriangles)
	{
		for (var i = 0; i < _topEdgeVerticesIndices.Count; i++)
		{
			var topIndex = _topEdgeVerticesIndices[i];
			var bottomIndex = _bottomEdgeVerticesIndices[i] + (combinedVertices.Count / 2); // Adjust for offset

			// Weld vertices by averaging their positions
			var weldedPosition = (combinedVertices[topIndex] + combinedVertices[bottomIndex]) / 2;
			combinedVertices[topIndex] = weldedPosition;
			combinedVertices[bottomIndex] = weldedPosition;
		}

		for (var i = 0; i < _topEdgeVerticesIndices.Count; i++)
		{
			var nextIndex = (i + 1) % _topEdgeVerticesIndices.Count;

			var topVertexIndex1 = _topEdgeVerticesIndices[i];
			var topVertexIndex2 = _topEdgeVerticesIndices[nextIndex];
			var bottomVertexIndex1 = _bottomEdgeVerticesIndices[i];
			var bottomVertexIndex2 = _bottomEdgeVerticesIndices[nextIndex];

			// Correctly orient the triangles
			combinedTriangles.Add(topVertexIndex1);
			combinedTriangles.Add(topVertexIndex2);
			combinedTriangles.Add(bottomVertexIndex1);

			combinedTriangles.Add(bottomVertexIndex1);
			combinedTriangles.Add(topVertexIndex2);
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
				var xPos = ((x / Resolution) - 0.5f) * 2 * Radius;
				var yPos = ((y / Resolution) - 0.5f) * 2 * Radius;

				// Check if vertex is on the edge
				var pos = (xPos * xPos) + (yPos * yPos);

				if ((xPos * xPos) + (yPos * yPos) >= EdgeVertexThreshold)
				{
					_topEdgeVerticesIndices.Add(i);
				}

				if ((xPos * xPos) + (yPos * yPos) <= Radius * Radius) // Inside the circle
				{
					var height = Mathf.PerlinNoise(x * TopNoiseScale, y * TopNoiseScale) * TopNoiseHeight;
					var distanceFromEdge = Radius - Mathf.Sqrt((xPos * xPos) + (yPos * yPos));

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
				var xPos = ((x / Resolution) - 0.5f) * 2 * Radius;
				var yPos = ((y / Resolution) - 0.5f) * 2 * Radius;

				// Check if vertex is on the edge
				if ((xPos * xPos) + (yPos * yPos) >= EdgeVertexThreshold)
				{
					_bottomEdgeVerticesIndices.Add(i);
				}

				if ((xPos * xPos) + (yPos * yPos) <= Radius * Radius) // Inside the circle
				{
					var distanceFromCenter = Mathf.Sqrt((xPos * xPos) + (yPos * yPos));
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
		return (vertices[index].x * vertices[index].x) + (vertices[index].z * vertices[index].z) <= rad * rad &&
				 (vertices[index + 1].x * vertices[index + 1].x) + (vertices[index + 1].z * vertices[index + 1].z) <= rad * rad &&
				 (vertices[index + res].x * vertices[index + res].x) + (vertices[index + res].z * vertices[index + res].z) <= rad * rad &&
				 (vertices[index + res + 1].x * vertices[index + res + 1].x) + (vertices[index + res + 1].z * vertices[index + res + 1].z) <= rad * rad;
	}
}
