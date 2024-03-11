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
		var area = (Mathf.PI * Mathf.Pow(_islandSpawnRadius, 2)) - (Mathf.PI * Mathf.Pow(_outerRadius, 2));
		var maxIslands = Mathf.FloorToInt(area / Mathf.Pow(IslandSizeOffset * 2, 2));
		var totalIslands = Mathf.FloorToInt(maxIslands * (_biome.IslandsFrequency / 100f));
		for (var i = 0; i < totalIslands; i++)
		{
			var randomPoint = PlacerUtils.RandomPointWithinAnnulus(_randomGenerator, _centerPosition, _outerRadius, _islandSpawnRadius);
			if (!Physics.CheckSphere(randomPoint, IslandSizeOffset, ObjectPlacer.ObstacleLayerMask | ObjectPlacer.GroundLayerMask))
			{
				Spawn(randomPoint);
			}
		}
	}

	void Spawn(Vector3 position)
	{
		var prefab = _biome.IslandsPrefabs[_randomGenerator.Next(0, _biome.IslandsPrefabs.Length)];
		var rotation = Quaternion.Euler(0, _randomGenerator.NextFloat(0, 360), 0);
		var scale = _randomGenerator.NextFloat(_biome.IslandScaleRange.x, _biome.IslandScaleRange.y);

		var island = Object.Instantiate(prefab, position, rotation, _parentObject);
		island.transform.localScale = new Vector3(scale, scale, scale);
		island.layer = ObjectPlacer.ObstacleLayer;
		PlacerUtils.SetColor(island, _biome.Material);
	}
}
