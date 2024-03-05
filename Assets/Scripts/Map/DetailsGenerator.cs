using UnityEngine;

public class DetailsGenerator
{
	readonly Biome _biome;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle");
	readonly float _outerRadius;
	readonly float _innerRadius;

	public DetailsGenerator(Biome biome, Transform parentObject, Vector3 centerPosition, float outerRadius, float innerRadius)
	{
		_biome = biome;
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
			var randomPoint = RandomPointWithinAnnulus(_centerPosition, startRadius, radius);
			if (IsPointValid(randomPoint))
			{
				SpawnDetail(randomPoint);
			}
		}
	}

	Vector3 RandomPointWithinAnnulus(Vector3 center, float minRadius, float maxRadius)
	{
		var direction = Random.insideUnitCircle.normalized;
		var distance = Random.Range(minRadius, maxRadius);
		return new Vector3(center.x + (direction.x * distance), 0, center.z + (direction.y * distance));
	}

	bool IsPointValid(Vector3 point)
	{
		if (Physics.CheckSphere(point, 0.5f, _obstacleLayerMask))
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
		var prefab = _biome.DetailsPrefabs[Random.Range(0, _biome.DetailsPrefabs.Length)];
		var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
		_ = Object.Instantiate(prefab, position, rotation, _parentObject);
	}
}
