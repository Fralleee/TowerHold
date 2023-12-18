using System;
using UnityEngine;

[Serializable]
public class SplinePoint
{
	public Vector3 Position;
	public float Width = 1f;

	public SplinePoint(Vector3 position, float width)
	{
		Position = position;
		Width = width;
	}
}
