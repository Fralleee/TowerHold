using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator
{
	readonly Biome _biome;
	readonly RandomGenerator _randomGenerator;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle");
	readonly float _outerRadius;
	readonly float _innerRadius;

	readonly Collider[] _collidersBuffer = new Collider[1];

	public ForestGenerator(Biome biome, RandomGenerator randomGenerator, Transform parentObject, Vector3 centerPosition, float outerRadius, float innerRadius)
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
		var totalSeeds = Mathf.CeilToInt(_randomGenerator.NextFloat(_biome.SeedRange.x, _biome.SeedRange.y));

		while (totalSeeds > 0)
		{
			var randomDirection = _randomGenerator.InsideUnitCircle().normalized;
			var randomDistance = _randomGenerator.NextFloat(_innerRadius, _outerRadius);
			var seedPoint = _centerPosition + new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;

			if (IsPointValid(seedPoint))
			{
				GrowForestFromSeed(seedPoint);
				totalSeeds--;
			}
		}
	}

	bool IsPointValid(Vector3 point)
	{
		var isWithinRange = Vector3.Distance(_centerPosition, point) >= _innerRadius && Vector3.Distance(_centerPosition, point) <= _outerRadius;
		if (!isWithinRange)
		{
			return false;
		}

		var isNoiseValid = Mathf.PerlinNoise((point.x * _biome.TreeNoiseScale) + _centerPosition.x, point.z * _biome.TreeNoiseScale + _centerPosition.z) > _biome.TreeNoiseThreshold;
		if (!isNoiseValid)
		{
			return false;
		}

		var colliders = Physics.OverlapSphereNonAlloc(point, _biome.DistanceBetweenTrees / 2, _collidersBuffer, _obstacleLayerMask);
		if (colliders > 0)
		{
			return false;
		}

		var isNotNearObstacle = !Physics.CheckSphere(point, _biome.DistanceBetweenTrees / 2, _obstacleLayerMask);
		if (!isNotNearObstacle)
		{
			return false;
		}

		if (Physics.Raycast(point + (Vector3.up * 50f), Vector3.down, out _, Mathf.Infinity, _obstacleLayerMask))
		{
			return false;
		}

		var isNotNearGroundEdge = !PlacerUtils.IsNearGroundEdge(point, _biome.DistanceBetweenTrees);
		if (!isNotNearGroundEdge)
		{
			return false;
		}

		return true;
	}

	void GrowForestFromSeed(Vector3 seedPoint)
	{
		var growthPoints = new Queue<Vector3>();
		var placedTrees = new HashSet<Vector3>();
		growthPoints.Enqueue(seedPoint);
		_ = placedTrees.Add(seedPoint);

		var forestLength = _randomGenerator.NextFloat(_biome.ForestLengthRange.x, _biome.ForestLengthRange.y);

		while (growthPoints.Count > 0 && placedTrees.Count < forestLength)
		{
			var currentPoint = growthPoints.Dequeue();
			for (var i = 0; i < 8; i++)
			{
				var angle = i * 45;
				var offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * _biome.DistanceBetweenTrees;
				var nextPoint = currentPoint + offset;
				if (!placedTrees.Contains(nextPoint) && IsPointValid(nextPoint))
				{
					PlaceTree(nextPoint);
					growthPoints.Enqueue(nextPoint);
					_ = placedTrees.Add(nextPoint);
				}
			}
		}
	}

	void PlaceTree(Vector3 position)
	{
		var prefab = _biome.TreePrefabs[_randomGenerator.Next(0, _biome.TreePrefabs.Length)];
		var rotation = Quaternion.Euler(0, _randomGenerator.NextFloat(0, 360), 0);
		var scale = _randomGenerator.NextFloat(_biome.TreeScaleRange.x, _biome.TreeScaleRange.y);

		var tree = Object.Instantiate(prefab, position, rotation, _parentObject);
		tree.transform.localScale = new Vector3(scale, scale, scale);

		tree.GetComponentInChildren<Renderer>().sharedMaterial = _biome.Material;
	}
}
