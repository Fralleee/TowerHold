using UnityEngine;

public static class GizmosExtras
{
	public static void Draw2dCircle(Vector3 center, float radius)
	{
		var prevPos = center + new Vector3(radius, 0, 0);
		for (var i = 0; i < 30; i++)
		{
			var angle = i / 30f * Mathf.PI * 2f;
			var newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
			Gizmos.DrawLine(prevPos, newPos);
			prevPos = newPos;
		}
	}
}
