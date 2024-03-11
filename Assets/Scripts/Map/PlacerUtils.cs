using System.Collections.Generic;
using UnityEngine;

public static class PlacerUtils
{
	const float RaycastStartHeight = 0.1f;
	const float RaycastDistance = 10f;
	const float MapBevelOffset = 5f;

	static readonly Vector3[] FourDirections = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
	static readonly Vector3[] EightDirections =
	{
		Vector3.forward,
		(Vector3.forward + Vector3.right).normalized,
		Vector3.right,
		(Vector3.right + Vector3.back).normalized,
		Vector3.back,
		(Vector3.back + Vector3.left).normalized,
		Vector3.left,
		(Vector3.left + Vector3.forward).normalized
	};

	// Check if a position is near the edge of the ground within a specified threshold
	public static bool IsNearGroundEdge(Vector3 position, float edgeThreshold, DirectionCount directionCount = DirectionCount.Four)
	{
		var directions = directionCount == DirectionCount.Four ? FourDirections : EightDirections;
		foreach (var dir in directions)
		{
			var checkPosition = position + (Vector3.up * RaycastStartHeight) + (dir * (edgeThreshold + MapBevelOffset));
			if (!Physics.Raycast(checkPosition, Vector3.down, RaycastDistance, LayerMask.GetMask("Ground")))
			{
				return true; // Edge detected in this direction
			}
		}
		return false; // No edges detected, not near ground edge
	}


	public static bool IsPointObstructed(Vector3 point, float checkRadius, LayerMask obstacleLayerMask, DirectionCount directionCount = DirectionCount.Four)
	{
		var directions = directionCount == DirectionCount.Four ? FourDirections : EightDirections;
		foreach (var dir in directions)
		{
			if (Physics.SphereCast(point, checkRadius, dir, out _, checkRadius, obstacleLayerMask))
			{
				return true; // Obstruction found
			}
		}
		return false; // No obstructions
	}

	public static bool IsSplinePointObstructed(Vector3 point, float width, float minDistanceBetweenPoints, List<SplinePoint> allSplinePoints)
	{
		foreach (var splinePoint in allSplinePoints)
		{
			if (Vector3.Distance(point, splinePoint.Position) < width + splinePoint.Width + minDistanceBetweenPoints)
			{
				return true; // Found an obstruction
			}
		}
		return false; // No obstructions found
	}

	public static Vector3 RandomPointWithinAnnulus(RandomGenerator randomGenerator, Vector3 center, float minRadius, float maxRadius)
	{
		var direction = randomGenerator.InsideUnitCircle().normalized;
		var distance = randomGenerator.NextFloat(minRadius, maxRadius);
		return new Vector3(center.x + (direction.x * distance), 0, center.z + (direction.y * distance));
	}

	public static void SetColor(GameObject gameObject, Material material)
	{
		gameObject.GetComponentInChildren<Renderer>().sharedMaterial = material;
	}
}


public enum DirectionCount
{
	Four,
	Eight
}
