using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RiversGenerator
{
	readonly Biome _biome;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly List<SplinePoint> _allSplinePoints = new List<SplinePoint>();
	readonly float _outerRadius;
	readonly float _innerRadius;

	public RiversGenerator(Biome biome, Transform parentObject, Vector3 centerPosition, float outerRadius, float innerRadius)
	{
		_biome = biome;
		_parentObject = parentObject;
		_centerPosition = centerPosition;
		_outerRadius = outerRadius;
		_innerRadius = innerRadius;
	}

	public void Generate()
	{
		_allSplinePoints.Clear();

		var riversCreated = 0;
		var maxAttemptsPerRiver = 10;
		while (riversCreated < _biome.TotalRivers && maxAttemptsPerRiver > 0)
		{
			var spawnPoint = PlacerUtils.RandomPointWithinAnnulus(_centerPosition, _innerRadius, _outerRadius);
			if (IsPointValid(spawnPoint, _biome.RiverWidthRange.y))
			{
				var river = InstantiateRiver(_biome.RiverPrefab, spawnPoint);
				var successful = SetupSplineMeshGenerator(river);
				if (successful)
				{
					riversCreated++;
					maxAttemptsPerRiver = 10;
				}
				else
				{
					maxAttemptsPerRiver--;
					Object.DestroyImmediate(river);
				}
			}
		}

		InstantiateSea();
	}

	void InstantiateSea()
	{
		var sea = GameObject.CreatePrimitive(PrimitiveType.Plane);
		sea.transform.SetParent(_parentObject);
		sea.transform.localScale = new Vector3(500, 1, 500);
		sea.transform.localPosition = Vector3.down * 2;

		var renderer = sea.GetComponentInChildren<Renderer>();
		renderer.material = _biome.RiverPrefab.GetComponentInChildren<Renderer>().sharedMaterial;

		var propBlock = new MaterialPropertyBlock();

		renderer.GetPropertyBlock(propBlock);

		propBlock.SetColor("_ColorShallow", _biome.RiverColorShallow);
		propBlock.SetColor("_ColorDeep", _biome.RiverColorDeep);

		renderer.SetPropertyBlock(propBlock);

	}

	GameObject InstantiateRiver(GameObject prefab, Vector3 position)
	{
		var river = Object.Instantiate(prefab, position + Vector3.up, Quaternion.identity, _parentObject);

		var propBlock = new MaterialPropertyBlock();
		var renderer = river.GetComponentInChildren<Renderer>();

		renderer.GetPropertyBlock(propBlock);

		propBlock.SetColor("_ColorShallow", _biome.RiverColorShallow);
		propBlock.SetColor("_ColorDeep", _biome.RiverColorDeep);

		renderer.SetPropertyBlock(propBlock);

		return river;
	}


	bool IsPointValid(Vector3 point, float width)
	{
		var isOuteSideInnerRadius = (Vector3.Distance(_centerPosition, point) - width) >= _innerRadius;
		if (!isOuteSideInnerRadius)
		{
			return false;
		}

		var isWithinRange = (Vector3.Distance(_centerPosition, point) + width) <= _outerRadius;
		if (!isWithinRange)
		{
			return false;
		}

		var isNotSplinePointObstructed = !PlacerUtils.IsSplinePointObstructed(point, width, _biome.MinDistanceBetweenOtherPoints, _allSplinePoints);
		if (!isNotSplinePointObstructed)
		{
			return false;
		}

		var isNotNearGroundEdge = !PlacerUtils.IsNearGroundEdge(point, width * 3);
		if (!isNotNearGroundEdge)
		{
			return false;
		}


		return true;
	}

	void AddFirstPoint(SplineMeshGenerator splineMeshGenerator, Vector3 riverPosition)
	{
		var width = Random.Range(_biome.RiverWidthRange.x, _biome.RiverWidthRange.y);

		var splinePoint = new SplinePoint(Vector3.zero, width);
		splineMeshGenerator.SplinePoints.Add(splinePoint);

		var splinePointGlobal = new SplinePoint(riverPosition, width);
		_allSplinePoints.Add(splinePointGlobal);
	}

	bool SetupSplineMeshGenerator(GameObject river)
	{
		if (river.TryGetComponent<SplineMeshGenerator>(out var splineMeshGenerator))
		{
			splineMeshGenerator.SplinePoints.Clear();

			var pointsCount = Random.Range(_biome.PointsPerRiverRange.x, _biome.PointsPerRiverRange.y + 1);
			var lastPoint = Vector3.zero;

			AddFirstPoint(splineMeshGenerator, river.transform.position);
			pointsCount--;

			var noiseOffset = Random.Range(0f, 100f);

			for (var i = 0; i < pointsCount; i++)
			{
				var angle = Mathf.PerlinNoise(noiseOffset, i * _biome.RiversNoiseScale) * 360f;
				var direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
				direction.Normalize();

				var distance = Random.Range(_biome.DistanceBetweenPointsRange.x, _biome.DistanceBetweenPointsRange.y);
				var nextPoint = lastPoint + (direction * distance);
				var globalPoint = nextPoint + river.transform.position;
				var width = Random.Range(_biome.RiverWidthRange.x, _biome.RiverWidthRange.y);
				if (!IsPointValid(globalPoint, width))
				{
					continue;
				}

				var splinePoint = new SplinePoint(nextPoint, width);
				splineMeshGenerator.SplinePoints.Add(splinePoint);

				var splinePointGlobal = new SplinePoint(globalPoint, width);
				_allSplinePoints.Add(splinePointGlobal);

				lastPoint = nextPoint;
			}

			if (splineMeshGenerator.SplinePoints.Count < 2)
			{
				return false;
			}

			splineMeshGenerator.UpdateMesh();
			return true;
		}

		return false;
	}

}
