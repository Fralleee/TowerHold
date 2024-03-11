using System.Collections.Generic;
using UnityEngine;

public class RoadsGenerator
{
	readonly Biome _biome;
	readonly GameObject _ground;
	readonly Transform _parentObject;
	readonly Vector3 _centerPosition;
	readonly List<SplinePoint> _allSplinePoints = new List<SplinePoint>();

	public RoadsGenerator(Biome biome, GameObject ground, Transform parentObject, Vector3 centerPosition)
	{
		_biome = biome;
		_ground = ground;
		_parentObject = parentObject;
		_centerPosition = centerPosition;
	}

	void SetGroundColor()
	{
		var propBlock = new MaterialPropertyBlock();
		var renderer = _ground.GetComponentInChildren<Renderer>();

		renderer.GetPropertyBlock(propBlock);

		propBlock.SetColor("_BaseColor", _biome.GroundColor);

		renderer.SetPropertyBlock(propBlock);
	}

	public void Generate()
	{
		_allSplinePoints.Clear();
		SetGroundColor();
		var road = Spawn();
		SetupSplineMeshGenerator(road);
	}

	void AddSplinePoint(SplineMeshGenerator splineMeshGenerator, Vector3 position)
	{
		var splinePoint = new SplinePoint(position, _biome.RoadWidth);
		splineMeshGenerator.SplinePoints.Add(splinePoint);
	}

	void SetupSplineMeshGenerator(GameObject road)
	{
		if (road.TryGetComponent<SplineMeshGenerator>(out var splineMeshGenerator))
		{
			splineMeshGenerator.SplinePoints.Clear();


			AddSplinePoint(splineMeshGenerator, Vector3.zero);
			AddSplinePoint(splineMeshGenerator, Vector3.forward);

			splineMeshGenerator.UpdateMesh();
		}
	}

	GameObject Spawn()
	{
		var road = Object.Instantiate(_biome.RoadPrefab, _centerPosition + (Vector3.up * 0.01f), Quaternion.identity, _parentObject);

		var propBlock = new MaterialPropertyBlock();
		var renderer = road.GetComponentInChildren<Renderer>();

		renderer.GetPropertyBlock(propBlock);

		propBlock.SetColor("_BaseColor", _biome.RoadColor);

		renderer.SetPropertyBlock(propBlock);

		return road;
	}
}
