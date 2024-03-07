using UnityEngine;

public class IslandGenerator
{
	const float IslandSizeOffset = 16f;

	readonly Biome _biome;
	readonly RandomGenerator _randomGenerator;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly float _outerRadius;
	readonly float _islandSpawnRadius;
	readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle");
	readonly LayerMask _groundLayerMask = LayerMask.GetMask("Ground");

	public IslandGenerator(Biome biome, RandomGenerator randomGenerator, Transform parentObject, Vector3 centerPosition, float outerRadius)
	{
		_biome = biome;
		_randomGenerator = randomGenerator;
		_parentObject = parentObject;
		_centerPosition = centerPosition;
		_outerRadius = outerRadius + IslandSizeOffset;
		_islandSpawnRadius = _outerRadius + _biome.IslandDistanceFromOuterRadius;
	}

	public void Generate()
	{
		// Calculate the maximum possible islands in the area, considering a minimum distance between them
		var area = (Mathf.PI * Mathf.Pow(_islandSpawnRadius, 2)) - (Mathf.PI * Mathf.Pow(_outerRadius, 2));
		var maxIslands = Mathf.FloorToInt(area / Mathf.Pow(IslandSizeOffset * 2, 2));
		var totalIslands = Mathf.FloorToInt(maxIslands * (_biome.IslandsFrequency / 100f));
		for (var i = 0; i < totalIslands; i++)
		{
			var randomPoint = RandomPointInAnnulus(_centerPosition, _outerRadius, _islandSpawnRadius);
			if (!Physics.CheckSphere(randomPoint, IslandSizeOffset, _obstacleLayerMask | _groundLayerMask))
			{
				SpawnIsland(randomPoint);
			}
		}
	}

	Vector3 RandomPointInAnnulus(Vector3 center, float minRadius, float maxRadius)
	{
		var angle = _randomGenerator.NextFloat(0, 360) * Mathf.Deg2Rad;
		var radius = _randomGenerator.NextFloat(minRadius, maxRadius);
		return new Vector3(
			center.x + (radius * Mathf.Cos(angle)),
			center.y,
			center.z + (radius * Mathf.Sin(angle))
		);
	}

	void SpawnIsland(Vector3 position)
	{
		var prefab = _biome.IslandsPrefabs[_randomGenerator.Next(0, _biome.IslandsPrefabs.Length)];
		var rotation = Quaternion.Euler(0, _randomGenerator.NextFloat(0, 360), 0);
		var scale = _randomGenerator.NextFloat(_biome.IslandScaleRange.x, _biome.IslandScaleRange.y);

		var island = Object.Instantiate(prefab, position, rotation, _parentObject);
		island.transform.localScale = new Vector3(scale, scale, scale);

		island.GetComponentInChildren<Renderer>().sharedMaterial = _biome.Material;
	}
}
