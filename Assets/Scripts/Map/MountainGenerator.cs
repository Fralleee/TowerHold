using UnityEngine;

public class MountainGenerator
{
	const float MountainSizeOffset = 10f;

	readonly Biome _biome;
	readonly RandomGenerator _randomGenerator;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly float _outerRadius;
	readonly float _innerRadius;

	public MountainGenerator(Biome biome, RandomGenerator randomGenerator, Transform parentObject, Vector3 position, float outerRadius, float innerRadius)
	{
		_biome = biome;
		_randomGenerator = randomGenerator;
		_parentObject = parentObject;
		_centerPosition = position;
		_outerRadius = outerRadius;
		_innerRadius = innerRadius;
	}

	public void Generate()
	{
		for (var i = 0; i < Mathf.CeilToInt((_outerRadius - _innerRadius) * _biome.MountainFrequency); i++)
		{
			GenerateMountainCluster();
		}
	}

	void GenerateMountainCluster()
	{
		var randomPoint = PlacerUtils.RandomPointWithinAnnulus(_randomGenerator, _centerPosition, _innerRadius + MountainSizeOffset, _outerRadius);
		var mountainsInCluster = _randomGenerator.Next(1, 4);
		for (var j = 0; j < mountainsInCluster; j++)
		{
			GenerateSingleMountain(randomPoint);
		}
	}

	void GenerateSingleMountain(Vector3 clusterCenter)
	{
		var offsetX = (Mathf.PerlinNoise(clusterCenter.x * _biome.MountainNoiseScale, clusterCenter.z * _biome.MountainNoiseScale) - 0.5f) * MountainSizeOffset;
		var offsetZ = (Mathf.PerlinNoise(clusterCenter.z * _biome.MountainNoiseScale, clusterCenter.x * _biome.MountainNoiseScale) - 0.5f) * MountainSizeOffset;
		var mountainPosition = clusterCenter + new Vector3(offsetX, 0, offsetZ);
		if (IsPointValid(mountainPosition))
		{
			Spawn(mountainPosition);
		}
	}

	bool IsPointValid(Vector3 point)
	{
		var isWithinRange = Vector3.Distance(_centerPosition, point) <= _outerRadius;
		if (!isWithinRange)
		{
			return false;
		}

		var isNotNearObstacle = !Physics.CheckSphere(point, 10f, ObjectPlacer.ObstacleLayerMask);
		if (!isNotNearObstacle)
		{
			return false;
		}

		var isNotNearGroundEdge = !PlacerUtils.IsNearGroundEdge(point, MountainSizeOffset);
		if (!isNotNearGroundEdge)
		{
			return false;
		}

		return true;
	}

	void Spawn(Vector3 position)
	{
		var prefab = _biome.MountainPrefabs[_randomGenerator.Next(0, _biome.MountainPrefabs.Length)];
		var rotation = Quaternion.Euler(0, _randomGenerator.NextFloat(0, 360), 0);
		var scale = _randomGenerator.NextFloat(_biome.MountainScaleRange.x, _biome.MountainScaleRange.y);
		var mountain = Object.Instantiate(prefab, position, rotation, _parentObject);
		mountain.transform.localScale = new Vector3(scale, scale, scale);
		mountain.SetLayerRecursively(ObjectPlacer.ObstacleLayer);
		PlacerUtils.SetColor(mountain, _biome.Material);
	}
}
