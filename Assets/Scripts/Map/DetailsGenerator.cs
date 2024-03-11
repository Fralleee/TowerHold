using UnityEngine;

public class DetailsGenerator
{
	readonly Biome _biome;
	readonly RandomGenerator _randomGenerator;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly float _outerRadius;
	readonly float _innerRadius;

	public DetailsGenerator(Biome biome, RandomGenerator randomGenerator, Transform parentObject, Vector3 centerPosition, float outerRadius, float innerRadius)
	{
		_biome = biome;
		_randomGenerator = randomGenerator;
		_parentObject = parentObject;
		_centerPosition = centerPosition;
		_outerRadius = outerRadius;
		_innerRadius = innerRadius;
	}

	public void Generate()
	{
		SpawnDetailsInArea(_innerRadius, _biome.DetailsInnerFrequency);
		SpawnDetailsInArea(_outerRadius, _biome.DetailsOuterFrequency, _innerRadius);
	}

	void SpawnDetailsInArea(float radius, float frequency, float startRadius = 0f)
	{
		var totalAttempts = Mathf.CeilToInt(radius * frequency);
		for (var i = 0; i < totalAttempts; i++)
		{
			var randomPoint = PlacerUtils.RandomPointWithinAnnulus(_randomGenerator, _centerPosition, startRadius, radius);
			if (IsPointValid(randomPoint))
			{
				SpawnDetail(randomPoint);
			}
		}
	}

	bool IsPointValid(Vector3 point)
	{
		if (Physics.CheckSphere(point, 0.5f, ObjectPlacer.ObstacleLayerMask))
		{
			return false;
		}

		if (PlacerUtils.IsNearGroundEdge(point, 2f))
		{
			return false;
		}

		var noiseValue = Mathf.PerlinNoise(point.x * _biome.DetailsNoiseScale, point.z * _biome.DetailsNoiseScale);
		return noiseValue > _biome.DetailsNoiseThreshold;
	}

	void SpawnDetail(Vector3 position)
	{
		var prefab = _biome.DetailsPrefabs[_randomGenerator.Next(0, _biome.DetailsPrefabs.Length)];
		var rotation = Quaternion.Euler(0, _randomGenerator.NextFloat(0, 360), 0);
		var detail = Object.Instantiate(prefab, position, rotation, _parentObject);
		detail.layer = ObjectPlacer.ObstacleLayer;
	}
}
