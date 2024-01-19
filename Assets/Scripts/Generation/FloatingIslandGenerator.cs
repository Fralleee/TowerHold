using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FloatingIslandGenerator : MonoBehaviour
{
	public int MapWidth = 256;
	public int MapHeight = 256;
	public float MapScale = 20f;

	Texture2D _topHeightMap;
	Texture2D _bottomHeightMap;

	Mesh _islandMesh;

	void Start()
	{

		GenerateIsland();
	}

	[Button]
	void GenerateIsland()
	{
		_islandMesh = new Mesh();
		GetComponent<MeshFilter>().mesh = _islandMesh;

		// Generate the height maps
		_topHeightMap = GenerateHeightMap();
		_bottomHeightMap = GenerateHeightMap(isBottomMap: true);

		// Generate the top and bottom meshes using the height maps
		var (topVertices, bottomVertices) = GenerateVerticesFromHeightMaps(_topHeightMap, _bottomHeightMap);

		// Generate triangles for top and bottom meshes
		var topTriangles = GenerateTriangles(MapWidth, MapHeight);
		var bottomTriangles = GenerateTriangles(MapWidth, MapHeight, topVertices.Length);

		// Identify edge vertices and find their neighbors
		var isEdgeVertex = IdentifyEdgeVertices(MapWidth, MapHeight * 2); // *2 for top and bottom
		var vertexNeighbors = FindVertexNeighbors(MapWidth, MapHeight * 2); // *2 for top and bottom

		// Create side vertices and triangles
		var sideVertices = CreateSideVertices(topVertices, bottomVertices, isEdgeVertex, MapWidth, MapHeight);
		var sideTriangles = GenerateSideTriangles(sideVertices, MapWidth, MapHeight);

		// Combine top, bottom, and side vertices
		var totalVertices = new Vector3[topVertices.Length + bottomVertices.Length + sideVertices.Count];
		topVertices.CopyTo(totalVertices, 0);
		bottomVertices.CopyTo(totalVertices, topVertices.Length);
		sideVertices.CopyTo(totalVertices, topVertices.Length + bottomVertices.Length);

		// Displace vertices using Simplex Noise
		DisplaceVertices(totalVertices);

		// Smooth edges with the AVG Algorithm
		SmoothEdges(totalVertices, isEdgeVertex, vertexNeighbors);

		// Calculate normals and apply Normal Smoothing
		var combinedTriangles = new List<int>(topTriangles);
		combinedTriangles.AddRange(bottomTriangles);
		combinedTriangles.AddRange(sideTriangles);
		var normals = CalculateSmoothNormals(totalVertices, combinedTriangles.ToArray());

		// Apply Tri-planar projection texturing technique
		// TODO: Implement this

		_islandMesh.vertices = totalVertices;
		_islandMesh.triangles = combinedTriangles.ToArray();
		_islandMesh.normals = normals;
	}

	(Vector3[], Vector3[]) GenerateVerticesFromHeightMaps(Texture2D topMap, Texture2D bottomMap)
	{
		var width = topMap.width;
		var height = topMap.height;

		if (bottomMap.width != width || bottomMap.height != height)
		{
			Debug.LogError("Top and bottom height maps must be of the same size.");
			return (null, null);
		}

		var topVertices = new Vector3[width * height];
		var bottomVertices = new Vector3[width * height];

		var scaleFactor = 10.0f;
		var yOffsetTop = 0.0f;
		var yOffsetBottom = -5.0f;

		// Introduce a small vertical offset between the two meshes
		var verticalOffset = 3f; // Adjust this value as needed

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var index = x + (y * width);
				var topHeight = topMap.GetPixel(x, y).grayscale;
				var bottomHeight = bottomMap.GetPixel(x, y).grayscale;

				topVertices[index] = new Vector3(x, (topHeight * scaleFactor) + yOffsetTop, y);
				bottomVertices[index] = new Vector3(x, ((1.0f - bottomHeight) * scaleFactor) + yOffsetBottom - verticalOffset, y);
			}
		}

		return (topVertices, bottomVertices);
	}

	void DisplaceVertices(Vector3[] vertices)
	{
		var noiseScale = 0.1f; // Adjust this scale to change the noise frequency
		var displacementScale = 5.0f; // Adjust this scale to change the maximum displacement

		for (var i = 0; i < vertices.Length; i++)
		{
			var noiseValue = Mathf.PerlinNoise(vertices[i].x * noiseScale, vertices[i].z * noiseScale);
			vertices[i].y += noiseValue * displacementScale;
		}
	}

	Vector3[] CalculateSmoothNormals(Vector3[] vertices, int[] triangles)
	{
		var normals = new Vector3[vertices.Length];
		var triangleNormals = new Vector3[triangles.Length / 3];

		// Calculate normals for each triangle
		for (var i = 0; i < triangles.Length; i += 3)
		{
			var p1 = vertices[triangles[i]];
			var p2 = vertices[triangles[i + 1]];
			var p3 = vertices[triangles[i + 2]];

			var normal = Vector3.Cross(p2 - p1, p3 - p1).normalized;
			var triangleIndex = i / 3;
			triangleNormals[triangleIndex] = normal;

			normals[triangles[i]] += normal;
			normals[triangles[i + 1]] += normal;
			normals[triangles[i + 2]] += normal;
		}

		// Normalize the vertex normals
		for (var i = 0; i < normals.Length; i++)
		{
			normals[i] = normals[i].normalized;
		}

		return normals;
	}

	int[] GenerateTriangles(int width, int height, int vertexOffset = 0)
	{
		var triangles = new int[(width - 1) * (height - 1) * 6];
		var triangleIndex = 0;

		for (var y = 0; y < height - 1; y++)
		{
			for (var x = 0; x < width - 1; x++)
			{
				var bottomLeft = (y * width) + x + vertexOffset;
				var bottomRight = bottomLeft + 1;
				var topLeft = bottomLeft + width;
				var topRight = topLeft + 1;

				// Top mesh or Bottom mesh with correct winding order
				var order = vertexOffset == 0 ? new int[] { topLeft, bottomRight, bottomLeft, topLeft, topRight, bottomRight }
												: new int[] { bottomLeft, bottomRight, topLeft, bottomRight, topRight, topLeft };

				for (var i = 0; i < 6; i++)
				{
					triangles[triangleIndex++] = order[i];
				}
			}
		}

		return triangles;
	}

	Texture2D GenerateHeightMap(bool isBottomMap = false)
	{
		var heightMap = new Texture2D(MapWidth, MapHeight);

		for (var x = 0; x < MapWidth; x++)
		{
			for (var y = 0; y < MapHeight; y++)
			{
				float sample;
				if (isBottomMap)
				{
					sample = CalculateBottomMapValue(x, y, MapWidth, MapHeight);
				}
				else
				{
					var xCoord = (float)x / MapWidth * MapScale;
					var yCoord = (float)y / MapHeight * MapScale;
					sample = Mathf.PerlinNoise(xCoord, yCoord);
				}

				var color = new Color(sample, sample, sample);
				heightMap.SetPixel(x, y, color);
			}
		}

		heightMap.Apply();
		return heightMap;
	}

	float CalculateBottomMapValue(int x, int y, int width, int height)
	{
		// Calculate distance from the center
		var centerX = width / 2f;
		var centerY = height / 2f;
		var distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));

		// Normalize distance
		var maxDistance = Mathf.Sqrt((centerX * centerX) + (centerY * centerY));
		var normalizedDistance = distance / maxDistance;

		// Invert and scale the distance to create a fade-out effect
		var fadeOutValue = 1.0f - normalizedDistance;
		fadeOutValue = Mathf.Clamp(fadeOutValue, 0f, 1f);

		// Combine fade-out with Perlin noise for variation
		var noiseValue = Mathf.PerlinNoise((float)x / width * MapScale, (float)y / height * MapScale);
		return (1.0f - noiseValue) * fadeOutValue;
	}

	bool[] IdentifyEdgeVertices(int width, int height)
	{
		var totalVertices = width * height * 2; // *2 for top and bottom
		var isEdgeVertex = new bool[totalVertices];

		for (var i = 0; i < totalVertices; i++)
		{
			var x = i % width;
			var y = i / width % height;
			var isTopLayer = i < width * height;

			isEdgeVertex[i] = x == 0 || y == 0 || x == width - 1 || y == height - 1;
			if (isTopLayer)
			{
				// Additional logic for top layer if needed
			}
			else
			{
				// Additional logic for bottom layer if needed
			}
		}

		return isEdgeVertex;
	}

	List<int>[] FindVertexNeighbors(int width, int height)
	{
		var totalVertices = width * height * 2; // *2 for top and bottom meshes only
		var vertexNeighbors = new List<int>[totalVertices];
		for (var i = 0; i < totalVertices; i++)
		{
			vertexNeighbors[i] = new List<int>();
			var layerOffset = i < width * height ? 0 : width * height;  // Determine if it's a top or bottom layer
			var x = i % width;
			var y = i / width % height;

			for (var dy = -1; dy <= 1; dy++)
			{
				for (var dx = -1; dx <= 1; dx++)
				{
					if (dx == 0 && dy == 0)
					{
						continue; // Skip the vertex itself
					}

					var neighborX = x + dx;
					var neighborY = y + dy;

					if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
					{
						var neighborIndex = neighborY * width + neighborX + layerOffset;
						if (neighborIndex < totalVertices) // Ensure the index is within bounds
						{
							vertexNeighbors[i].Add(neighborIndex);
						}
					}
				}
			}
		}
		return vertexNeighbors;

	}




	void SmoothEdges(Vector3[] vertices, bool[] isEdgeVertex, List<int>[] vertexNeighbors)
	{
		var smoothedVertices = new Vector3[vertices.Length];

		for (var i = 0; i < vertices.Length; i++)
		{
			if (isEdgeVertex[i])
			{
				var averagePosition = Vector3.zero;
				var neighborCount = 0;

				foreach (var neighborIndex in vertexNeighbors[i])
				{
					averagePosition += vertices[neighborIndex];
					neighborCount++;
				}

				if (neighborCount > 0)
				{
					averagePosition /= neighborCount;
					smoothedVertices[i] = averagePosition;
				}
				else
				{
					smoothedVertices[i] = vertices[i];
				}
			}
			else
			{
				smoothedVertices[i] = vertices[i];
			}
		}

		// Update original vertices with smoothed positions
		for (var i = 0; i < vertices.Length; i++)
		{
			vertices[i] = smoothedVertices[i];
		}
	}

	List<Vector3> CreateSideVertices(Vector3[] topVertices, Vector3[] bottomVertices, bool[] isEdgeVertex, int width, int height)
	{
		var sideVertices = new List<Vector3>();

		// Top edge vertices
		for (var i = 0; i < width * height; i++)
		{
			if (isEdgeVertex[i]) // If it's an edge vertex
			{
				sideVertices.Add(topVertices[i]); // Add top vertex
				sideVertices.Add(bottomVertices[i]); // Add corresponding bottom vertex directly below
			}
		}

		return sideVertices;
	}


	int[] GenerateSideTriangles(List<Vector3> sideVertices, int width, int height)
	{
		var sideTriangles = new List<int>();

		// Iterate over the edge vertices to create quads (two triangles per quad)
		for (var i = 0; i < sideVertices.Count; i += 2)
		{
			if (i + 3 < sideVertices.Count) // Check to avoid out-of-range
			{
				// First triangle
				sideTriangles.Add(i);
				sideTriangles.Add(i + 1);
				sideTriangles.Add(i + 2);

				// Second triangle
				sideTriangles.Add(i + 2);
				sideTriangles.Add(i + 1);
				sideTriangles.Add(i + 3);
			}
		}

		return sideTriangles.ToArray();
	}

}
